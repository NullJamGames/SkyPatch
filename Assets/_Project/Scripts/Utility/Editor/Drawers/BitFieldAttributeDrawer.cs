using System;
using UnityEditor;
using UnityEngine;

namespace NJG.Utilities.Editor
{
    /// <summary>
    ///     Draws an enum as an Editor MaskField
    /// </summary>
    [CustomPropertyDrawer(typeof(BitFieldAttribute))]
    public sealed class BitFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                Type type = Enum.GetUnderlyingType(fieldInfo.FieldType);

                if (type == typeof(sbyte) || type == typeof(short) || type == typeof(int))
                {
                    EditorGUI.BeginProperty(position, label, property);
                    property.intValue =
                        EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
                    EditorGUI.EndProperty();

                    return;
                }
            }

            EditorGUI.LabelField(position, label.text, "Unsupported field type.");
        }
    }
}