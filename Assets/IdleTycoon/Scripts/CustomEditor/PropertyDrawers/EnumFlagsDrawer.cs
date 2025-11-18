#if UNITY_EDITOR
using IdleTycoon.Scripts.CustomEditor.Attributes;
using UnityEditor;
using UnityEngine;

namespace IdleTycoon.Scripts.CustomEditor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public sealed class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(
                position,
                label,
                property.intValue,
                property.enumDisplayNames
            );
        }
    }
}
#endif