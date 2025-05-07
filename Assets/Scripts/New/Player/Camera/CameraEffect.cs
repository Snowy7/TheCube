using UnityEngine;

namespace New.Player
{
// Base class for camera effects
    public abstract class CameraEffect : MonoBehaviour
    {
        public abstract void ApplyEffect(Transform cameraTransform);
    }
}