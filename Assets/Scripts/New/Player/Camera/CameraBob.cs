using UnityEngine;

namespace New.Player
{
    // Camera bob effect (for walking/running)
    [System.Serializable]
    public class CameraBob : CameraEffect
    {
        [SerializeField] private float walkBobAmount = 0.05f;
        [SerializeField] private float walkBobSpeed = 14f;
        [SerializeField] private float sprintBobAmount = 0.08f;
        [SerializeField] private float sprintBobSpeed = 18f;
        
        private float bobTimer;
        private Vector3 initialPosition;
        private MovementSystem movementSystem;
        private CameraSystem cameraSystem;
        
        private void Start()
        {
            initialPosition = transform.localPosition;
            movementSystem = GetComponentInParent<MovementSystem>();
            cameraSystem = GetComponentInParent<CameraSystem>();
        }
        
        public override void ApplyEffect(Transform cameraTransform)
        {
            if (movementSystem == null) return;
            
            if (movementSystem.IsGrounded && 
                (movementSystem.CurrentState == MovementState.Normal || 
                 movementSystem.CurrentState == MovementState.Sprinting))
            {
                // Only bob when moving on ground
                bool isMoving = movementSystem.IsMoving;
                if (isMoving)
                {
                    // Calculate bob parameters based on state
                    float bobAmount = movementSystem.IsSprinting ? sprintBobAmount : walkBobAmount;
                    float bobSpeed = movementSystem.IsSprinting ? sprintBobSpeed : walkBobSpeed;
                    
                    // Increase timer
                    bobTimer += Time.deltaTime * bobSpeed;
                    
                    // Apply vertical bob using sine wave
                    float yOffset = Mathf.Sin(bobTimer) * bobAmount;
                    
                    // Apply horizontal bob using cosine wave with half the frequency
                    float xOffset = Mathf.Cos(bobTimer / 2) * bobAmount / 2;
                    
                    // Apply bob
                    Vector3 bobPosition = new Vector3(initialPosition.x + xOffset, initialPosition.y + yOffset, initialPosition.z);
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, bobPosition, Time.deltaTime * 4f);
                }
                else
                {
                    // Reset when not moving
                    bobTimer = 0;
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialPosition, Time.deltaTime * 4f);
                }
            }
            else
            {
                // Reset when not in normal movement state
                bobTimer = 0;
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialPosition, Time.deltaTime * 4f);
            }
        }
    }
}