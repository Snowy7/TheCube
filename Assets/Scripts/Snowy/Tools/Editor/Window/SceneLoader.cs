using System;
using System.Collections.Generic;
using SnowyEditor.Gui;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnowyEditor.Window
{
    public enum SceneLoaderMode
    {
        AllScenes,
        BuildScenes,
        FavoriteScenes
    } 
    
    /// <summary>
    /// A Window that shows all the scenes in the project and allows the user to load them.
    /// With editable functionality so you can pin your favorite scenes. or search for a scene.
    /// </summary>
    public class SceneLoader : EditorWindow
    {
        # region Fields
        
        // flag for the scene loader mode
        private SceneLoaderMode _sceneLoaderMode = SceneLoaderMode.BuildScenes;
        
        // The search string
        private string _searchString = "";
        
        # endregion
        
        # region Variables

        // The list of all scenes in the project
        private string[] _scenes;
        
        private string _favoriteScenes;
        private SceneLoaderMode _previousSceneLoaderMode;
        
        private Vector2 _scrollPos;
        
        # endregion
        
        // Load all scenes in the project on start
        private void OnEnable()
        {
            // Clear the search string
            _searchString = "";

            // Load all the settings from the editor prefs
            _sceneLoaderMode = (SceneLoaderMode)EditorPrefs.GetInt("SceneLoaderMode", 0);
            _favoriteScenes = EditorPrefs.GetString("FavoriteScenes", "");
            
            // Load the scenes based on the mode
            switch (_sceneLoaderMode)
            {
                case SceneLoaderMode.AllScenes:
                    LoadAllScenes();
                    break;
                case SceneLoaderMode.BuildScenes:
                    LoadBuildScenes();
                    break;
                case SceneLoaderMode.FavoriteScenes:
                    LoadFavoriteScenes();
                    break;
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Scene Loader", SnGUISkin.titleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            
            // Draw the scene loader mode
            _sceneLoaderMode = (SceneLoaderMode)EditorGUILayout.EnumPopup("Scene Loader Mode", _sceneLoaderMode);
            
            if (_sceneLoaderMode != _previousSceneLoaderMode)
            {
                _previousSceneLoaderMode = _sceneLoaderMode;
                EditorPrefs.SetInt("SceneLoaderMode", (int)_sceneLoaderMode);
                switch (_sceneLoaderMode)
                {
                    case SceneLoaderMode.AllScenes:
                        LoadAllScenes();
                        break;
                    case SceneLoaderMode.BuildScenes:
                        LoadBuildScenes();
                        break;
                    case SceneLoaderMode.FavoriteScenes:
                        LoadFavoriteScenes();
                        break;
                }
            }
            
            // Draw the search bar
            _searchString = EditorGUILayout.TextField("Search", _searchString);
            
            // Draw the scroll view
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (var scene in _scenes)
            {
                if (string.IsNullOrEmpty(scene)) continue;
                // if the scene name contains the search string
                if (scene.ToLower().Contains(_searchString.ToLower()))
                {
                    DrawSceneButton(scene);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneButton(string scene)
        {
            // draw the scene button and the favorite button
            EditorGUILayout.BeginHorizontal();
            // get only scene name
            var sceneName = scene.Substring(scene.LastIndexOf('/') + 1).Replace(".unity", "");
            
            if (GUILayout.Button(new GUIContent(sceneName, scene), GUILayout.Height(30)))
            {
                LoadScene(scene);
            }
                    
            var sceneGuid = AssetDatabase.AssetPathToGUID(scene);
            var isFavorite = _favoriteScenes.Contains(sceneGuid);
            var favoriteButton = isFavorite ? "★" : "☆";
            if (GUILayout.Button(favoriteButton, GUILayout.Width(30)))
            {
                if (isFavorite)
                {
                    _favoriteScenes = _favoriteScenes.Replace($"{sceneGuid},", "");
                    if (_sceneLoaderMode == SceneLoaderMode.FavoriteScenes)
                    {
                        LoadFavoriteScenes();
                    }
                }
                else
                {
                    if (_favoriteScenes.Contains(sceneGuid)) return;
                    _favoriteScenes += $"{sceneGuid},";
                }
                EditorPrefs.SetString("FavoriteScenes", _favoriteScenes);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void OnProjectChange()
        {
            switch (_sceneLoaderMode)
            {
                case SceneLoaderMode.AllScenes:
                    LoadAllScenes();
                    break;
                case SceneLoaderMode.BuildScenes:
                    LoadBuildScenes();
                    break;
                case SceneLoaderMode.FavoriteScenes:
                    LoadFavoriteScenes();
                    break;
            }
        }

        void LoadAllScenes()
        {
            // search for all scenes in the project
            var guids = AssetDatabase.FindAssets("t:Scene");
            _scenes = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                _scenes[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            
            // sort by favorites
            var favoriteScenes = new List<string>();
            var otherScenes = new List<string>();
            foreach (var scene in _scenes)
            {
                if (_favoriteScenes.Contains(AssetDatabase.AssetPathToGUID(scene)))
                {
                    favoriteScenes.Add(scene);
                }
                else
                {
                    otherScenes.Add(scene);
                }
            }
            
            favoriteScenes.AddRange(otherScenes);
            _scenes = favoriteScenes.ToArray();
        }

        void LoadBuildScenes()
        {
            _scenes = new string[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }
        }
        
        void LoadFavoriteScenes()
        {
            // Load all the scenes from the editor prefs
            var guids = EditorPrefs.GetString("FavoriteScenes", "");
            if (string.IsNullOrEmpty(guids))
            {
                _scenes = new string[0];
                return;
            }
            var sceneGuids = guids.Split(',');
            _scenes = new string[sceneGuids.Length];
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                if (string.IsNullOrEmpty(sceneGuids[i])) continue;
                _scenes[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            }
        }
        
        /// <summary>
        /// Load the scene
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            { 
                // Load the scene in play mode
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // If need saving ask whether to save or no, or cancel
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // Load the scene in edit mode
                    EditorSceneManager.OpenScene(sceneName);
                }
            }
        }
    }
}