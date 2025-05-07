using System;
using Toolbox.Editor;
using Toolbox.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace SnowyEditor.Inspectors
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    internal class MonoBehaviourEditor : Editor<MonoBehaviour>, IToolboxEditor
    {
        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            ToolboxEditorHandler.HandleToolboxEditor(this);
        }

        /// <inheritdoc />
        [Obsolete("To draw properties in a different way override the Drawer property.")]
        public virtual void DrawCustomProperty(SerializedProperty property)
        { }

        /// <inheritdoc />
        public virtual void DrawCustomInspector()
        {
            Drawer.DrawEditor(serializedObject);
        }

        /// <inheritdoc />
        public void IgnoreProperty(SerializedProperty property)
        {
            Drawer.IgnoreProperty(property);
        }

        /// <inheritdoc />
        public void IgnoreProperty(string propertyPath)
        {
            Drawer.IgnoreProperty(propertyPath);
        }

        Editor IToolboxEditor.ContextEditor => this;
        /// <inheritdoc />
        public virtual IToolboxEditorDrawer Drawer { get; } = new ToolboxEditorDrawer();

#pragma warning disable 0067
        [Obsolete("ToolboxEditorHandler.OnBeginToolboxEditor")]
        public static event Action<Editor> OnBeginToolboxEditor;
        [Obsolete("ToolboxEditorHandler.OnBreakToolboxEditor")]
        public static event Action<Editor> OnBreakToolboxEditor;
        [Obsolete("ToolboxEditorHandler.OnCloseToolboxEditor")]
        public static event Action<Editor> OnCloseToolboxEditor;
#pragma warning restore 0067
    }
}
