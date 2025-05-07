using System;
using Snowy.CSharp;
using Snowy.Inspector;
using SnowyEditor.Gui;
using UnityEditor;
using UnityEngine;

namespace SnowyEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    internal class SortingLayerIDDrawer : PropertyDrawer
    {
        private SortingLayerDrawTool _tool;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_tool == null)
            {
                if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
                {
                    EditorGui.ErrorLabel(position, label, $"Use {nameof(SortingLayerIDAttribute)} with int.");
                    return;
                }

                _tool = new SortingLayerDrawTool();
            }

            property.intValue = _tool.Draw(position, label.text, property.intValue);
        }
    }
}
