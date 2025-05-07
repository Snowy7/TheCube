using SnowyEditor.Gui;
using UnityEditor;
using UnityEngine;

namespace Plugins.Snowy.SnLevelDesigner
{
    [CustomEditor(typeof(SnLevelDesigner))]
    public class SnLevelDesignerEditor : Editor
    {
        
        [MenuItem("Tools/Snowy/Level Designer")]
        static void Init()
        {
            GameObject newObject = new GameObject("Level Designer");
            var levelDesigner = newObject.AddComponent<SnLevelDesigner>();
            // create the geometry container
            GameObject geometryContainer = new GameObject("Geometry");
            levelDesigner.SetGeometryRoot(geometryContainer);
        }
        
        private SnLevelDesigner levelDesigner;

        public override void OnInspectorGUI()
        {
            levelDesigner = (SnLevelDesigner) target;
            DrawDefaultScriptField();
            SnEditorGUI.DrawTitle("Level Designer");
            DrawFields();

            // spawn wall
            if (GUILayout.Button("Spawn Pillar"))
            {
                levelDesigner.CreateInstance(levelDesigner.tileSet.pillarTitle, levelDesigner.transform.position, Quaternion.identity);
            }
            
            // floor
            if (GUILayout.Button("Spawn Floor"))
            {
                levelDesigner.CreateInstance(levelDesigner.tileSet.floorTile, levelDesigner.transform.position, Quaternion.identity);
            }
            
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelDesigner);
            }
            
            Repaint();
        }
        
        private void DrawDefaultScriptField()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(levelDesigner), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawFields()
        {
            SnEditorGUI.BeginSection("Settings");

            // tile set
            levelDesigner.tileSet = (SnTileSet) EditorGUILayout.ObjectField("Tile Set", levelDesigner.tileSet, typeof(SnTileSet), false);
            
            // tile size
            levelDesigner.tileSize = EditorGUILayout.FloatField("Tile Size", levelDesigner.tileSize);
            
            // spawn walls
            levelDesigner.spawnWalls = EditorGUILayout.Toggle("Spawn Walls", levelDesigner.spawnWalls);
            
            // spawn floors
            levelDesigner.spawnFloors = EditorGUILayout.Toggle("Spawn Floors", levelDesigner.spawnFloors);
            
            // spawn pillars
            levelDesigner.spawnPillars = EditorGUILayout.Toggle("Spawn Pillars", levelDesigner.spawnPillars);
            
            SnEditorGUI.EndSection();
        }

        private void OnSceneGUI()
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Space)
                    {
                        levelDesigner.ToggleTool();
                    }

                    break;
            }
        }
    }
}