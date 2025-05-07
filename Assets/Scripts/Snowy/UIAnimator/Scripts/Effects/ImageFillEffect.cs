using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Snowy.UI.Effects.Effects
{
    [Serializable]
    public class ImageFillEffect : SnEffect
    {
        [SerializeField] float to = 1f;
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool forceFrom;
        [SerializeField, ShowIf(nameof(forceFrom), true)] float fFrom;
        
        Image m_image;
        private float m_originalFillAmount;
        
        public override void Initialize(IEffectsManager manager)
        {
            base.Initialize(manager);

            var image = customGraphicTarget && graphicTarget ? graphicTarget : manager.TargetGraphic;
            if (image == null) return;
            if (image is Image img)
            {
                m_image = img;
                
                m_originalFillAmount = m_image.fillAmount;
            }
        }
        
        public override IEnumerator Apply(IEffectsManager manager)
        {
            if (m_image == null) yield break;
            
            IsPlaying = true;
            float from = forceFrom ? fFrom : m_image?.fillAmount ?? 0f;
            
            float time = 0f;
            while (time < duration)
            {
                m_image.fillAmount = Mathf.Lerp(from, to, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
            m_image.fillAmount = to;
            IsPlaying = false;
        }
        
        public override IEnumerator Cancel(IEffectsManager manager)
        {
            IsPlaying = true;
            float time = 0f;
            var from = m_image.fillAmount;
            while (time < duration)
            {
                m_image.fillAmount = Mathf.Lerp(from, m_originalFillAmount, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
            
            m_image.fillAmount = m_originalFillAmount;
            
            IsPlaying = false;
        }
        
        public override void ImmediateCancel(IEffectsManager manager)
        {
            m_image.fillAmount = fFrom;
            IsPlaying = false;
        }
    }
}