using System.Collections;
using UnityEngine;

namespace Snowy.UI.Effects.Effects
{
    public class CanvasGroupFade : SnEffect
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField, Range(0, 1)] float to = 1f;
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool forceFrom;
        [SerializeField, ShowIf(nameof(forceFrom), true), Range(0, 1)] float fFrom;
        
        private float m_originalAlpha;
        
        public override void Initialize(IEffectsManager manager)
        {
            base.Initialize(manager);
            
            if (canvasGroup == null)
            {
                Debug.LogWarning("No canvas group assigned to the effect, creating a new one.");
                canvasGroup = graphicTarget.gameObject.AddComponent<CanvasGroup>();
            }
            
            m_originalAlpha = canvasGroup.alpha;
        }
        
        public override IEnumerator Apply(IEffectsManager manager)
        {
            // crossfade
            IsPlaying = true;
            if (forceFrom)
                canvasGroup.alpha = fFrom;
            
            // smoothly fade to the target alpha
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, to, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = to;
            IsPlaying = false;
        }

        public override IEnumerator Cancel(IEffectsManager manager)
        {
            // crossfade
            IsPlaying = true;
            
            // smoothly fade back to the original alpha
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, m_originalAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = m_originalAlpha;
            IsPlaying = false;
        }

        public override void ImmediateCancel(IEffectsManager manager)
        {
            canvasGroup.alpha = m_originalAlpha;
        }
    }
}