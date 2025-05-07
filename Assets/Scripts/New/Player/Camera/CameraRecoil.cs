using UnityEngine;

namespace New.Player
{
    // Camera recoil effect
    public class CameraRecoil : CameraEffect
    {
        private Vector2 recoilAmount;
        private float recoilDuration;
        private float recoilStartTime;
        private Vector2 targetRecoil;
        private Vector2 currentRecoil;
        
        public void ApplyRecoil(Vector2 amount, float duration)
        {
            recoilAmount = amount;
            recoilDuration = duration;
            recoilStartTime = Time.time;
            targetRecoil = amount;
            
            enabled = true;
        }
        
        public override void ApplyEffect(Transform cameraTransform)
        {
            if (Time.time - recoilStartTime > recoilDuration)
            {
                currentRecoil = Vector2.zero;
                enabled = false;
                return;
            }
            
            float progress = (Time.time - recoilStartTime) / recoilDuration;
            float recoilProgress = 1f - progress;
            
            // Apply recoil
            currentRecoil = Vector2.Lerp(Vector2.zero, targetRecoil, recoilProgress);
            
            // Update camera rotation - apply pitch change
            Vector3 currentEulerAngles = cameraTransform.localEulerAngles;
            float newPitch = NormalizeAngle(currentEulerAngles.x) - currentRecoil.y;
            newPitch = Mathf.Clamp(newPitch, -80f, 80f);
            
            // Apply yaw change to parent (player)
            cameraTransform.parent.Rotate(Vector3.up, -currentRecoil.x * Time.deltaTime);
            
            // Apply pitch to camera
            cameraTransform.localEulerAngles = new Vector3(newPitch, currentEulerAngles.y, currentEulerAngles.z);
        }
        
        private float NormalizeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }
            return angle;
        }
    }
}