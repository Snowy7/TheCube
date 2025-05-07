using UnityEngine;

namespace New.Player
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float slopeLimit = 45f;
        [SerializeField] private float groundedOffsetY = 0.05f; // Small offset for more reliable detection
        
        private CapsuleCollider playerCollider;
        
        private void Awake()
        {
            playerCollider = GetComponent<CapsuleCollider>();
        }
        
        public void Initialize(LayerMask layers, float checkDistance, float maxSlope)
        {
            groundLayers = layers;
            groundCheckDistance = checkDistance;
            slopeLimit = maxSlope;
        }
        
        public bool CheckGround(out Vector3 surfaceNormal)
        {
            surfaceNormal = Vector3.up;
            
            if (playerCollider == null)
            {
                playerCollider = GetComponent<CapsuleCollider>();
                if (playerCollider == null)
                {
                    Debug.LogError("GroundCheck: No CapsuleCollider found!");
                    return false;
                }
            }
            
            // Calculate the bottom center of the capsule
            Vector3 capsuleBottom = transform.position + 
                new Vector3(0, playerCollider.center.y - playerCollider.height / 2 + playerCollider.radius, 0);
            
            // Slightly raise the ray origin for more reliable ground detection
            Vector3 rayOrigin = capsuleBottom + Vector3.up * groundedOffsetY;
            
            // Perform the raycast
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 
                groundCheckDistance + groundedOffsetY, groundLayers))
            {
                surfaceNormal = hit.normal;
                
                // Check if slope is too steep
                float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);
                if (slopeAngle < slopeLimit)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw ground check ray for debugging
            if (playerCollider != null)
            {
                Vector3 capsuleBottom = transform.position + 
                    new Vector3(0, playerCollider.center.y - playerCollider.height / 2 + playerCollider.radius, 0);
                
                Vector3 rayOrigin = capsuleBottom + Vector3.up * groundedOffsetY;
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * (groundCheckDistance + groundedOffsetY));
            }
        }
    }
}
