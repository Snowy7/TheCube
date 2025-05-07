using System;
using Snowy.CSharp;
using Snowy.Inspector;
using UnityEditor;
using UnityEngine;

namespace SnowyEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(LayerFieldAttribute))]
    internal class LayerFieldDrawer : AttributeDrawer<LayerFieldAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int32)
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(LayerFieldAttribute)} with {nameof(Int32)}.");
                return;
            }

            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
