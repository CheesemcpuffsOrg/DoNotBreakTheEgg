using UnityEngine;
using UnityEditor;

public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionField;

    public ShowIfAttribute(string conditionField)
    {
        ConditionField = conditionField;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionField);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            bool isVisible = conditionProperty.boolValue;

            if (isVisible)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionField);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            return conditionProperty.boolValue ? EditorGUI.GetPropertyHeight(property, label) : 0;
        }

        return EditorGUI.GetPropertyHeight(property, label);
    }
}
#endif
