using UnityEngine;

namespace Snowy
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(Snowy) + "/Render Sorter")]
    public sealed class RenderSorter : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

#if UNITY_EDITOR
        internal Renderer Renderer => _renderer;
        internal static string RendererFieldName => nameof(_renderer);
#endif
    }
}
