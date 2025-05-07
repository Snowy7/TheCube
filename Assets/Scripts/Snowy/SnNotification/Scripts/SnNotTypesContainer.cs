using System;
using Snowy.Inspector;
using UnityEngine;

namespace SnNotification
{
    [Serializable] public class NotificationType
    {
        public string name;
        public int poolSize;
        public SnNotObject notificationObject;
        public bool hasLayoutGroup;
        [SerializeField, ShowIf(nameof(hasLayoutGroup), true)] public GameObject layoutGroupPrefab;
    }
    
    [CreateAssetMenu(fileName = "SnNotTypesContainer", menuName = "Snowy/SnNotify/SnNotTypesContainer")]
    public class SnNotTypesContainer : ScriptableObject
    {
        # if UNITY_EDITOR
        [EditorButton(nameof(BuildTypes))]
        # endif
        [SerializeField, ReorderableList] public NotificationType[] notificationTypes;
        
        # if UNITY_EDITOR
        public static string RootPath
        {
            get
            {
                var g = UnityEditor.AssetDatabase.FindAssets($"t:Script {nameof(SnNotTypesContainer)}");
                if (g.Length == 0)
                {
                    Debug.LogError("SnNotTypesContainer.cs not found in the project");
                    return null;
                }
                
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(g[0]);
                return path.Replace($"{nameof(SnNotTypesContainer)}.cs", "NotificationTypeNames.cs");
            }
        }
        
        public static void BuildNotificationTypesEnum(string[] notificationTypes) 
        {
            string enumString = "namespace SnNotification\n{\n\tpublic enum NotificationTypeNames {\n";
            for (int i = 0; i < notificationTypes.Length; i++)
            {
                enumString += $"\t\t{notificationTypes[i]},\n";
            }
            enumString += "\t}\n}";
            
            // Get current path
            // Write to file
            System.IO.File.WriteAllText(RootPath, enumString);
            
            // Refresh the editor
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("NotificationTypeNames.cs file created at: " + RootPath);
        }
        
        
        private void BuildTypes()
        {
            // Show a unity loader
            UnityEditor.EditorUtility.DisplayProgressBar("Building Notification Types", "Building...", 0);
            
            string[] notTypes = new string[notificationTypes.Length];
            
            for (int i = 0; i < notificationTypes.Length; i++)
            {
                notTypes[i] = notificationTypes[i].name;
                
                // Show a unity loader
                UnityEditor.EditorUtility.DisplayProgressBar("Building Notification Types", "Building...", (float)i / notificationTypes.Length);
            }
            
            BuildNotificationTypesEnum(notTypes);
            
            // Hide the unity loader
            UnityEditor.EditorUtility.ClearProgressBar();
        }
        # endif
    }
}