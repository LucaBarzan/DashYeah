using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string valueStr;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = property.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                valueStr = property.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                valueStr = property.floatValue.ToString("0.00");
                break;
            case SerializedPropertyType.String:
                valueStr = property.stringValue;
                break;
            case SerializedPropertyType.Enum:
                string[] enums = property.enumDisplayNames;
                if (property.enumValueIndex < 0 || property.enumValueIndex >= enums.Length)
                    valueStr = "Undefined";
                else
                    valueStr = enums[property.enumValueIndex];
                break;
            case SerializedPropertyType.Vector3:
                valueStr = "x: " + property.vector3Value.x + " y: " + property.vector3Value.y + " z: " + property.vector3Value.z;
                break;
            case SerializedPropertyType.Vector2:
                valueStr = "x: " + property.vector2Value.x + " y: " + property.vector2Value.y;
                break;
            case SerializedPropertyType.ObjectReference:
                if (property.objectReferenceValue == null)
                    valueStr = "null";
                else
                    valueStr = property.objectReferenceValue.ToString();
                break;
            default:
                valueStr = "(not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}
