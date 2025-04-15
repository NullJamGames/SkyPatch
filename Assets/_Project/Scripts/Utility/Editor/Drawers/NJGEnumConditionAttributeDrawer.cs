using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NJG.Utilities.Editor
{
    [CustomPropertyDrawer(typeof(NJGEnumConditionAttribute))]
    public class NJGEnumConditionAttributeDrawer : PropertyDrawer
    {
        private static Dictionary<string, string> _cachedPaths = new ();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            NJGEnumConditionAttribute enumConditionAttribute = (NJGEnumConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);
            bool previouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!enumConditionAttribute.Hidden || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = previouslyEnabled;
        }

        private bool GetConditionAttributeResult(NJGEnumConditionAttribute enumConditionAttribute, SerializedProperty property)
        {
            bool enabled = true;

            SerializedProperty enumProp;
            string enumPropPath = string.Empty;
            string propertyPath = property.propertyPath;

            if (!_cachedPaths.TryGetValue(propertyPath, out enumPropPath))
            {
                enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.ConditionEnum);
                _cachedPaths.Add(propertyPath, enumPropPath);
            }

            enumProp = property.serializedObject.FindProperty(enumPropPath);

            if (enumProp != null)
            {
                int currentEnum = enumProp.enumValueIndex;
                enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
            }
            else
            {
                Debug.LogWarning("No matching enum found for NJGEnumConditionAttribute in object: " + enumConditionAttribute.ConditionEnum);
            }

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            NJGEnumConditionAttribute enumConditionAttribute = (NJGEnumConditionAttribute)attribute;
            bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);

            if (!enumConditionAttribute.Hidden || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
