using UnityEngine;

namespace Utilities
{
    public class Rotate : MonoBehaviour
    {
        public Vector3 rotationSpeed;

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}