using System;
using System.IO;
using Snowy.Mathematics;
using SnowyEditor.Utils;
using UnityEngine;

namespace SnowyEditor.Configs
{
    [Serializable]
    internal class SnEditorUserSettings
    {
        private const string FILE_PATH = AssetDatabaseExt.USER_SETTINGS_FOLDER + LibConstants.LIB_NAME + "Settings.json";

        [SerializeField]
        private bool _openFoldersByDoubleClick = true;
        [SerializeField]
        private bool _openScriptableAssetsAsCode;
        [SerializeField]
        private int _namespaceFolderRootSkipSteps;
        [SerializeField]
        private string _editorFolderNamespace = "EditorCode";

        private static SnEditorUserSettings _instance;

        private static SnEditorUserSettings Instance => _instance ?? (_instance = Load());

        public static bool OpenFoldersByDoubleClick
        {
            get => Instance._openFoldersByDoubleClick;
            set => SetField(ref Instance._openFoldersByDoubleClick, value);
        }

        public static bool OpenScriptableAssetsAsCode
        {
            get => Instance._openScriptableAssetsAsCode;
            set => SetField(ref Instance._openScriptableAssetsAsCode, value);
        }

        public static int NamespaceFolderRootSkipSteps
        {
            get => Instance._namespaceFolderRootSkipSteps.ClampMin(0);
            set => SetField(ref Instance._namespaceFolderRootSkipSteps, value);
        }

        public static string EditorFolderNamespace
        {
            get => Instance._editorFolderNamespace;
            set => SetField(ref Instance._editorFolderNamespace, value);
        }

        private static void Save(SnEditorUserSettings instance)
        {
            if (!Directory.Exists(AssetDatabaseExt.USER_SETTINGS_FOLDER))
                Directory.CreateDirectory(AssetDatabaseExt.USER_SETTINGS_FOLDER);

            string json = JsonUtility.ToJson(instance, true);
            File.WriteAllText(FILE_PATH, json);
        }

        private static SnEditorUserSettings Load()
        {
            if (!File.Exists(FILE_PATH))
            {
                var instance = new SnEditorUserSettings();
                Save(instance);
                return instance;
            }
            else
            {
                string json = File.ReadAllText(FILE_PATH);
                return JsonUtility.FromJson<SnEditorUserSettings>(json);
            }
        }

        private static void SetField<T>(ref T field, T value) where T : IEquatable<T>
        {
            if (field.Equals(value))
                return;

            field = value;
            Save(Instance);
        }
    }
}
