using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Snowy.UI.Effects
{
    [Serializable] public abstract class SnEffect : IEffect
    {
        public bool customGraphicTarget;
        [ShowIf(nameof(customGraphicTarget), true)]public Graphic graphicTarget;
        
        public bool IsPlaying { get; protected set; }

        public virtual void Initialize(IEffectsManager manager)
        {
            if (customGraphicTarget && graphicTarget == null)
            {
                Debug.LogWarning("No graphic target assigned to the effect, using the manager's target graphic instead.");
                graphicTarget = manager.TargetGraphic;
            } else if (!customGraphicTarget)
            {
                graphicTarget = manager.TargetGraphic;
            }
        }
        
        public abstract IEnumerator Apply(IEffectsManager manager);
        
        public abstract IEnumerator Cancel(IEffectsManager manager);

        public abstract void ImmediateCancel(IEffectsManager manager);
        
        
    }
}