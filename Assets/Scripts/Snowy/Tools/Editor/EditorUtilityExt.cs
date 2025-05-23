﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Snowy.Collections;
using Snowy.CSharp;
using SnowyEditor.Window;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace SnowyEditor
{
    public static class EditorUtilityExt
    {
        public const string SCRIPT_FIELD = "m_Script";
        public const string ASSET_NAME_FIELD = "m_Name";

        private static MethodInfo _clearFunc;

        public static void OpenCsProject()
        {
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        }

        public static void ClearConsoleWindow()
        {
            if (_clearFunc == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type type = assembly.GetType("UnityEditor.LogEntries");
                _clearFunc = type.GetMethod("Clear");
            }
            _clearFunc.Invoke(null, null);
        }

        public static string ConvertToSystemTypename(string managedReferenceFieldTypename)
        {
            if (managedReferenceFieldTypename.IsNullOrEmpty())
                return string.Empty;

            string[] typeSplitString = managedReferenceFieldTypename.Split(' ');
            return $"{typeSplitString[1]}, {typeSplitString[0]}";
        }

        public static Type GetTypeFromSerializedPropertyTypename(string managedReferenceTypename)
        {
            if (managedReferenceTypename.IsNullOrEmpty())
                return null;

            return Type.GetType(ConvertToSystemTypename(managedReferenceTypename));
        }

        public static Type GetFieldType(PropertyDrawer drawer)
        {
            return GetFieldType(drawer.fieldInfo);
        }

        public static Type GetFieldType(FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;
            return fieldType.IsArray ? fieldType.GetElementType()
                                     : fieldType;
        }

        public static object FindContainer(FieldInfo targetField, object root)
        {
            Type rootType = root.GetType();

            IEnumerable<FieldInfo> fields = rootType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(item => item.IsPublic || item.IsDefined(typeof(SerializeField)) || item.IsDefined(typeof(SerializeReference)));
            foreach (FieldInfo field in fields)
            {
                if (targetField == field)
                    return root;

                if (GetFieldType(field).GetTypeCode() != TypeCode.Object)
                    continue;

                object result = FindContainer(targetField, field.GetValue(root));
                if (result != null)
                    return result;
            }

            return null;
        }

        public static void DisplayDropDownList(Vector2 position, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected)
        {
            DisplayDropDownList(new Rect(position, Vector2.zero), displayedOptions, checkEnabled, onItemSelected);
        }

        public static void DisplayDropDownList(in Rect buttonRect, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected)
        {
            DropDownWindow list = ScriptableObject.CreateInstance<DropDownWindow>();

            for (int i = 0; i < displayedOptions.Length; i++)
            {
                int index = i;
                list.AddItem(displayedOptions[i], checkEnabled(i), () => onItemSelected(index));
            }

            list.ShowMenu(buttonRect);
        }

        public static void DisplayMultiSelectableList(Vector2 position, BitList flags, string[] displayedOptions, Action<BitList> onChanged = null)
        {
            DisplayMultiSelectableList(new Rect(position, Vector2.zero), flags, displayedOptions, onChanged);
        }

        public static void DisplayMultiSelectableList(in Rect buttonRect, BitList flags, string[] displayedOptions, Action<BitList> onChanged = null)
        {
            DropDownWindow.CreateForFlags(buttonRect, flags, displayedOptions, onChanged);
        }

        //The functions based on https://gist.github.com/bzgeb/3800350
        //God save this guy
        public static bool GetDroppedObjects(in Rect position, out UnityObject[] result)
        {
            Event curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (position.Contains(curEvent.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (curEvent.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            result = DragAndDrop.objectReferences;
                            return true;
                        }
                    }
                    break;
            }

            result = null;
            return false;
        }

        public static void OpenScriptableObjectCode(ScriptableObject scriptableObject)
        {
            OpenCsProject();

            using (SerializedObject serializedObject = new SerializedObject(scriptableObject))
            {
                using (SerializedProperty prop = serializedObject.FindProperty(SCRIPT_FIELD))
                {
                    AssetDatabase.OpenAsset(prop.objectReferenceValue);
                }
            }
        }

        public static void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(Directory.CreateDirectory(path).FullName);
                return;
            }

            Debug.LogError("Folder not found.");
        }

        public static void ExecuteWithProgressBar(string title, string info, IEnumerator<float> iterator, Action onSuccess = null)
        {
            try
            {
                while (iterator.MoveNext())
                {
                    EditorUtility.DisplayProgressBar(title, info, iterator.Current);
                }
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }

            EditorUtility.ClearProgressBar();
            onSuccess?.Invoke();
        }

        public static void ExecuteWithProgressBarCancelable(string title, string info, IEnumerator<float> iterator, Action onSuccess = null, Action onCansel = null)
        {
            try
            {
                while (iterator.MoveNext())
                {
                    if (EditorUtility.DisplayCancelableProgressBar(title, info, iterator.Current))
                    {
                        finish(onCansel);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }

            finish(onSuccess);

            void finish(Action onFin)
            {
                EditorUtility.ClearProgressBar();
                onFin?.Invoke();
            }
        }

        public static void ExecuteWithProgressBar(string title, IEnumerator<(string info, float progress)> iterator, Action onSuccess = null)
        {
            try
            {
                while (iterator.MoveNext())
                {
                    var (info, progress) = iterator.Current;
                    EditorUtility.DisplayProgressBar(title, info, progress);
                }
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }

            EditorUtility.ClearProgressBar();
            onSuccess?.Invoke();
        }

        public static void ExecuteWithProgressBarCancelable(string title, IEnumerator<(string info, float progress)> iterator, Action onSuccess = null, Action onCansel = null)
        {
            try
            {
                while (iterator.MoveNext())
                {
                    var (info, progress) = iterator.Current;

                    if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
                    {
                        finish(onCansel);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }

            finish(onSuccess);

            void finish(Action onFin)
            {
                EditorUtility.ClearProgressBar();
                onFin?.Invoke();
            }
        }
    }
}
