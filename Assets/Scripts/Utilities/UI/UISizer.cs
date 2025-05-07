using System;
using UnityEngine;

namespace Utilities.UI
{
    public class UISizer : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 padding;
        [SerializeField] private Vector2 offset;
        [SerializeField] private bool fitWidth;
        [SerializeField] private bool fitHeight;
        [SerializeField] private float updateAfterSeconds;

        private void Start()
        {
            if (target == null) target = transform.parent.GetComponent<RectTransform>();
            
            Resize();
            
            if (target == null) Debug.LogError("Target RectTransform is not set.", this);
            
            if (updateAfterSeconds > 0)
            {
                Invoke(nameof(Resize), updateAfterSeconds);
            }
        }

        private void OnEnable()
        {
            Resize();
        }

        private void Resize()
        {
            var rect = GetComponent<RectTransform>();
            if (rect == null) return;
            var size = rect.sizeDelta;
            if (fitWidth) size.x = target.sizeDelta.x - padding.x;
            if (fitHeight) size.y = target.sizeDelta.y - padding.y;
            rect.sizeDelta = size + offset;
        }
    }
}