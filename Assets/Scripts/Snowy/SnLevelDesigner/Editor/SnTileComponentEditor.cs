using UnityEditor;
using UnityEngine;

namespace Plugins.Snowy.SnLevelDesigner
{
    [CustomEditor (typeof (SnTileComponent))]
    public class SnTileComponentEditor : Editor
    {
        public override void OnInspectorGUI () {
            DrawDefaultInspector ();

            SnTileComponent componentScript = (SnTileComponent) target;
            if (GUILayout.Button ("Convert to Wall")) {
                componentScript.ChangeComponent(componentScript.tileSet.wallTile);
            }
            if (GUILayout.Button ("Convert to Arch")) {
                componentScript.ChangeComponent (componentScript.tileSet.archTile);
            }
            if (GUILayout.Button ("Convert to Window")) {
                componentScript.ChangeComponent (componentScript.tileSet.windowTile);
            }
        }
    }
}