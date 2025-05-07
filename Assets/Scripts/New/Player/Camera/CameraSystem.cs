using UnityEngine;
using System.Collections.Generic;
using System;

namespace New.Player
{
    public class CameraSystem : MonoBehaviour
    {
        [Header("Look Settings")]
        [SerializeField] private float lookSensitivity = 1.0f;
        [SerializeField] private float verticalLookLimit = 80f;
        [SerializeField] private bool smoothLook = true;
        [SerializeField] private float lookSmoothTime = 0.1f;
        
        [Header("Effects")]
        [SerializeField] private List<CameraEffect> cameraEffects = new List<CameraEffect>();
        [SerializeField] private CameraShake shakeEffect;
        [SerializeField] private CameraRecoil recoilEffect;
        
        private float cameraPitch = 0f;
        private Vector2 lookVelocity;
        private Vector2 smoothLookInput;
        private Transform playerTransform;
        private Vector2 lookInput;
        
        private void Awake()
        {
            playerTransform = transform.parent;
            
            // Create default effects if not assigned
            if (shakeEffect == null)
            {
                shakeEffect = gameObject.AddComponent<CameraShake>();
                cameraEffects.Add(shakeEffect);
            }
            
            if (recoilEffect == null)
            {
                recoilEffect = gameObject.AddComponent<CameraRecoil>();
                cameraEffects.Add(recoilEffect);
            }
        }
        
        private void LateUpdate()
        {
            // Handle camera rotation
            HandleCameraRotation();
            
            // Apply camera effects
            foreach (var effect in cameraEffects)
            {
                if (effect.enabled)
                {
                    effect.ApplyEffect(transform);
                }
            }
        }
        
        public void HandleLookInput(Vector2 lookInput)
        {
            this.lookInput = lookInput;
        }
        
        public void ApplyShake(float intensity, float duration)
        {
            if (shakeEffect != null)
            {
                shakeEffect.Shake(intensity, duration);
            }
        }
        
        public void ApplyRecoil(Vector2 recoilAmount, float duration)
        {
            if (recoilEffect != null)
            {
                recoilEffect.ApplyRecoil(recoilAmount, duration);
            }
        }
        
        public void AddCameraEffect(CameraEffect effect)
        {
            if (!cameraEffects.Contains(effect))
            {
                cameraEffects.Add(effect);
            }
        }
        
        public void RemoveCameraEffect(CameraEffect effect)
        {
            cameraEffects.Remove(effect);
        }

        private void HandleCameraRotation()
        {
            // Smoothing for look input
            if (smoothLook)
            {
                smoothLookInput = Vector2.SmoothDamp(smoothLookInput, lookInput, ref lookVelocity, lookSmoothTime);
                lookInput = smoothLookInput;
            }
            
            // Horizontal rotation (around Y-axis) rotates the player
            playerTransform.Rotate(Vector3.up, lookInput.x * lookSensitivity * Time.deltaTime);
            
            // Vertical rotation (around X-axis) rotates the camera
            cameraPitch -= lookInput.y * lookSensitivity * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);
            
            // Apply pitch to camera
            transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        }
        
        // Properties
        public float LookSensitivity
        {
            get => lookSensitivity;
            set => lookSensitivity = value;
        }
        
        public Vector2 LookInput
        {
            get => lookInput;
            private set => lookInput = value;
        }
    }
}
