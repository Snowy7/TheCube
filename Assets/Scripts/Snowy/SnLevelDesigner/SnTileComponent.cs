using UnityEngine;

namespace Plugins.Snowy.SnLevelDesigner
{
    [ExecuteAlways]
    [SelectionBase]
    public class SnTileComponent : MonoBehaviour
    {
        public SnTileSet tileSet;
        
        public void ChangeComponent(SnTileComponent newComponent)
        {
            # if UNITY_EDITOR
            SnTileComponent newInstance = UnityEditor.PrefabUtility.InstantiatePrefab(newComponent) as SnTileComponent;
            if (newInstance != null)
            {
                newInstance.transform.position = transform.position;
                newInstance.transform.rotation = transform.rotation;
                newInstance.transform.localScale = transform.localScale;
                newInstance.tileSet = newComponent.tileSet;
                newInstance.transform.SetParent(transform.parent);
                DestroyImmediate(gameObject);
            }
            # endif
        }
    }
}