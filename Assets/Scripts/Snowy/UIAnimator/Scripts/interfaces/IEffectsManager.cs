using UnityEngine;
using UnityEngine.UI;

namespace Snowy.UI.Effects
{
    public interface IEffectsManager
    {
        public Transform Transform { get; }
        public Graphic TargetGraphic { get; }
        public MonoBehaviour Mono { get; }
    }
}