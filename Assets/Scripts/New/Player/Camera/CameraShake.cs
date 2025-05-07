using UnityEngine;

namespace New.Player
{
    // Camera shake effect
    public class CameraShake : CameraEffect
    {
        private float shakeIntensity;
        private float shakeDuration;
        private float shakeStartTime;
        private Vector3 originalPosition;
        
        public void Shake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeStartTime = Time.time;
            originalPosition = transform.localPosition;
            
            enabled = true;
        }
        
        public override void ApplyEffect(Transform cameraTransform)
        {
            if (Time.time - shakeStartTime > shakeDuration)
            {
                cameraTransform.localPosition = originalPosition;
                enabled = false;
                return;
            }
            
            float progress = (Time.time - shakeStartTime) / shakeDuration;
            float damping = 1f - progress;
            
            // Random shake offset
            float offsetX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * damping;
            float offsetY = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * damping;
            
            // Apply offset
            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0);
            cameraTransform.localPosition = originalPosition + shakeOffset;
        }
    }
}