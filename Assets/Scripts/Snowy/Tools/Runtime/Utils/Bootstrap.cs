using UnityEngine;

namespace Snowy
{
    public class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            // Load all the gameobjects in the resources/onload folder
            // then spawn them in the scene
            var assets = Resources.LoadAll("OnLoad", typeof(GameObject));
            foreach (var asset in assets)
            {
                var prefab = asset as GameObject;
                if (prefab != null)
                {
                    var prefabInstance = Object.Instantiate(prefab);
                }
            }
        }
    }
}