using UnityEngine;
using UnityEditor;

// This class contain custom drawer for ReadOnly attribute.
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    // Unity method for drawing GUI in Editor
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Saving previous GUI enabled value
        var previousGUIState = GUI.enabled;

        // Disabling edit for property
        GUI.enabled = false;

        // Drawing Property
        EditorGUI.PropertyField(position, property, label);

        // Setting old GUI enabled value
        GUI.enabled = previousGUIState;
    }
}