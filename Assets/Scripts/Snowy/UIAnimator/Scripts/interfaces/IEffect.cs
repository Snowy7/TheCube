using System.Collections;

namespace Snowy.UI.Effects
{
    public interface IEffect
    {
        bool IsPlaying { get; }
        void Initialize(IEffectsManager manager);
        IEnumerator Apply(IEffectsManager manager);
        IEnumerator Cancel(IEffectsManager manager);
        void ImmediateCancel(IEffectsManager manager); 
    }
}