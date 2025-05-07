using UnityEngine;

namespace New.Player
{
    [UnityEditor.CustomEditor(typeof(CameraSystem))]
    public class CameraSystemEditor : UnityEditor.Editor
    {
        private bool showLookSettings = true;
        private bool showEffects = true;
        
        public override void OnInspectorGUI()
        {
            CameraSystem cameraSystem = (CameraSystem)target;
            
            showLookSettings = UnityEditor.EditorGUILayout.Foldout(showLookSettings, "Look Settings");
            if (showLookSettings)
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("lookSensitivity"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalLookLimit"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothLook"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("lookSmoothTime"));
                UnityEditor.EditorGUI.indentLevel--;
            }
            
            showEffects = UnityEditor.EditorGUILayout.Foldout(showEffects, "Camera Effects");
            if (showEffects)
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraEffects"), true);
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("shakeEffect"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("recoilEffect"));
                
                UnityEditor.EditorGUILayout.Space();
                
                if (GUILayout.Button("Add Camera Bob Effect"))
                {
                    CameraBob bob = cameraSystem.gameObject.GetComponent<CameraBob>();
                    if (bob == null)
                    {
                        bob = cameraSystem.gameObject.AddComponent<CameraBob>();
                        cameraSystem.AddCameraEffect(bob);
                    }
                }
                
                UnityEditor.EditorGUI.indentLevel--;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}