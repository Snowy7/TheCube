using UnityEngine;
using System;

namespace New.Player
{
    /// <summary>
    /// Handles camera leaning (peeking) functionality for first-person gameplay.
    /// This system controls camera rotation and position offset when leaning.
    /// </summary>
    public class LeanSystem : MonoBehaviour
    {
        [Header("Lean Settings")]
        [Tooltip("Maximum lean angle in degrees")]
        [SerializeField] private float maxLeanAngle = 15f;
        
        [Tooltip("Horizontal position offset at maximum lean")]
        [SerializeField] private float leanPositionOffset = 0.5f;
        
        [Tooltip("How quickly the lean effect is applied")]
        [SerializeField] private float leanSpeed = 8f;
        
        [Tooltip("Whether leaning is currently enabled")]
        [SerializeField] private bool leaningEnabled = true;
        
        [Header("References")]
        [Tooltip("Camera transform to apply lean effects to (defaults to child camera)")]
        [SerializeField] private Transform cameraTransform;
        
        // Internal state
        private float currentLeanAngle = 0f;
        private float targetLeanAngle = 0f;
        private float currentLeanOffset = 0f;
        private float targetLeanOffset = 0f;
        private Vector3 originalCameraPosition;
        private Quaternion originalCameraRotation;
        
        // Events
        public event Action<float> OnLeanChanged; // Parameter is current lean angle
        
        private void Awake()
        {
            // Find camera if not assigned
            if (cameraTransform == null)
            {
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
                if (cameraTransform == null)
                {
                    Debug.LogError("LeanSystem: No camera transform assigned and no Camera component found in children!");
                    enabled = false;
                    return;
                }
            }
            
            // Store original position and rotation for reference
            originalCameraPosition = cameraTransform.localPosition;
            originalCameraRotation = cameraTransform.localRotation;
        }
        
        private void Update()
        {
            if (!leaningEnabled)
            {
                ResetLean();
                return;
            }
            
            UpdateLeanTransform();
        }
        
        private void UpdateLeanTransform()
        {
            // Smoothly interpolate to target lean values
            currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLeanAngle, leanSpeed * Time.deltaTime);
            currentLeanOffset = Mathf.Lerp(currentLeanOffset, targetLeanOffset, leanSpeed * Time.deltaTime);
            
            // Apply rotation - z-axis rotation for leaning
            Quaternion targetRotation = Quaternion.Euler(
                originalCameraRotation.eulerAngles.x,
                originalCameraRotation.eulerAngles.y,
                currentLeanAngle
            );
            cameraTransform.localRotation = targetRotation;
            
            // Apply position offset - move camera horizontally
            Vector3 newPosition = originalCameraPosition;
            newPosition.x += currentLeanOffset;
            cameraTransform.localPosition = newPosition;
            
            // Notify listeners of lean changes (only if significant change)
            if (Mathf.Abs(currentLeanAngle - targetLeanAngle) < 0.01f && 
                OnLeanChanged != null && 
                Mathf.Abs(targetLeanAngle) > 0.01f)
            {
                OnLeanChanged.Invoke(currentLeanAngle);
            }
        }
        
        /// <summary>
        /// Lean to the left
        /// </summary>
        public void LeanLeft()
        {
            if (!leaningEnabled) return;
            
            targetLeanAngle = maxLeanAngle;
            targetLeanOffset = -leanPositionOffset;
        }
        
        /// <summary>
        /// Lean to the right
        /// </summary>
        public void LeanRight()
        {
            if (!leaningEnabled) return;
            
            targetLeanAngle = -maxLeanAngle;
            targetLeanOffset = leanPositionOffset;
        }
        
        /// <summary>
        /// Reset lean to center position
        /// </summary>
        public void ResetLean()
        {
            targetLeanAngle = 0f;
            targetLeanOffset = 0f;
        }
        
        /// <summary>
        /// Enable or disable leaning functionality
        /// </summary>
        /// <param name="enabled">Whether leaning should be enabled</param>
        public void SetLeaningEnabled(bool enabled)
        {
            leaningEnabled = enabled;
            if (!leaningEnabled)
            {
                ResetLean();
            }
        }
        
        /// <summary>
        /// Set lean parameters
        /// </summary>
        /// <param name="maxAngle">Maximum lean angle in degrees</param>
        /// <param name="offset">Position offset at maximum lean</param>
        /// <param name="speed">Lean interpolation speed</param>
        public void SetLeanParameters(float maxAngle, float offset, float speed)
        {
            if (maxAngle > 0)
                maxLeanAngle = maxAngle;
                
            if (offset > 0)
                leanPositionOffset = offset;
                
            if (speed > 0)
                leanSpeed = speed;
        }
        
        /// <summary>
        /// Get the current lean angle
        /// </summary>
        public float CurrentLeanAngle => currentLeanAngle;
        
        /// <summary>
        /// Whether leaning is currently enabled
        /// </summary>
        public bool IsLeaningEnabled => leaningEnabled;
    }
}
