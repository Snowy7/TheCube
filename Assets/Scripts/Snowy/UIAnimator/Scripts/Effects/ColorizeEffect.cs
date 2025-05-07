using System.Collections;
using UnityEngine;

namespace Snowy.UI.Effects.Effects
{
    public class ColorizeEffect : SnEffect
    {
        [SerializeField] private Color color = Color.white;
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool forceFrom;
        [SerializeField, ShowIf(nameof(forceFrom), true)] private Color fFrom = Color.white;
        
        private Color m_originalColor;
        
        public override void Initialize(IEffectsManager manager)
        {
            base.Initialize(manager);

            m_originalColor = graphicTarget.color;
        }

        public override IEnumerator Apply(IEffectsManager manager)
        {
            // colorize
            IsPlaying = true;
            if (forceFrom)
                graphicTarget.color = fFrom;
            
            // Lerp
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                graphicTarget.color = Color.Lerp(graphicTarget.color, color, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            graphicTarget.color = color;
            
            IsPlaying = false;
        }
        
        public override IEnumerator Cancel(IEffectsManager manager)
        {
            // colorize
            IsPlaying = true;
            
            // Lerp
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                graphicTarget.color = Color.Lerp(graphicTarget.color, m_originalColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            graphicTarget.color = m_originalColor;
            
            IsPlaying = false;
        }
        
        public override void ImmediateCancel(IEffectsManager manager)
        {
            graphicTarget.color = m_originalColor;
        }
    }
}