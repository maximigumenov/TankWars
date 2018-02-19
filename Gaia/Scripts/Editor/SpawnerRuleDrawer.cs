using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gaia
{
    /*
    [CustomPropertyDrawer(typeof(SpawnCriteriaBase))]
    public class SpawnerRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("m_moobar"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
     */
}