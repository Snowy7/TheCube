using System;
using UnityEditor;
using UnityEngine;

namespace SnowyEditor.Window
{
    // Manage all the assets in the resources/onload folder
    // can create new prefabs, etc
    // can remove existing prefabs
    // can create runtime objects with specific scripts
    
    public class BootstrapWindow : EditorWindow
    {
        // assets in the resources/onload folder
        private UnityEngine.Object[] assets;
        
        private void OnEnable()
        {
            // Load all the assets in the resources/onload folder
            assets = Resources.LoadAll("OnLoad", typeof(GameObject));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Bootstrap Window");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Assets in the Resources/OnLoad folder");
            EditorGUILayout.Space();
            if (assets.Length == 0)
            {
                EditorGUILayout.LabelField("No assets in the Resources/OnLoad folder");
            }
            else
            {
                foreach (var asset in assets)
                {
                    RenderAsset(asset);
                }
            }
            
            // if the user selects a gameobject in the hierarchy, show a button to create a prefab from it
            if (Selection.activeGameObject != null)
            {
                if (GUILayout.Button("Create Prefab"))
                {
                    // create a prefab from the selected gameobject
                    var prefab = PrefabUtility.SaveAsPrefabAsset(Selection.activeGameObject, "Assets/Resources/OnLoad/" + Selection.activeGameObject.name + ".prefab");
                    // reload the assets
                    assets = Resources.LoadAll("OnLoad", typeof(GameObject));
                    
                    // remove the gameobject from the scene
                    DestroyImmediate(Selection.activeGameObject);
                    
                    // spawn the prefab in the scene
                    var prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    
                    // select the prefab instance in the hierarchy
                    Selection.activeGameObject = prefabInstance;
                    
                    // focus the scene view on the prefab instance
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }
            
            // show a button to create a new empty prefab
            if (GUILayout.Button("Create Empty Prefab"))
            {
                // create a new empty prefab
                var prefab = PrefabUtility.SaveAsPrefabAsset(new GameObject("New Prefab"), "Assets/Resources/OnLoad/NewPrefab.prefab");
                // reload the assets
                assets = Resources.LoadAll("OnLoad", typeof(GameObject));
            }
        }
        
        private void RenderAsset(UnityEngine.Object asset)
        {
            // render the asset preview, show the path as a tooltip, show an X button to remove the asset
            EditorGUILayout.BeginHorizontal();
            // render the asset preview
            EditorGUILayout.ObjectField(asset, typeof(GameObject), false);
            // show the path as a tooltip
            var path = AssetDatabase.GetAssetPath(asset);
            // show an X button to remove the asset
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                // remove the asset
                AssetDatabase.DeleteAsset(path);
                // reload the assets
                assets = Resources.LoadAll("OnLoad", typeof(GameObject));
            }
            EditorGUILayout.EndHorizontal();
            
        }
    }
}