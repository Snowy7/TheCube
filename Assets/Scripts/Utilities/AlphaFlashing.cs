using TMPro;
using UnityEngine;

namespace Utils
{
    public class AlphaFlashing : MonoBehaviour
    {
        [SerializeField] CanvasGroup group;
        [SerializeField] private float flashSpeed = 1f;
        [SerializeField] private float minAlpha = 0.5f;
        [SerializeField] private float maxAlpha = 1f;
        private float currentAlpha;
        
        private void Start()
        {
            currentAlpha = maxAlpha;
        }
        
        private void Update()
        {
            currentAlpha = Mathf.PingPong(Time.time * flashSpeed, maxAlpha - minAlpha) + minAlpha;
            group.alpha = currentAlpha;
        }
    }
}