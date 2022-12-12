using UnityEditor;
using UnityEngine;

using Parade;

public class MissingParadeSplineWindow : EditorWindow
{
    private const int WindowWidth = 410;
    private const int WindowHeight = 125;

    private GameObject currentSplinePrefab;

    private void OnEnable()
    {
        maxSize = new Vector2(WindowWidth, WindowHeight);
        minSize = new Vector2(WindowWidth, WindowHeight);
    }

    private void OnGUI()
    {
        GUILayout.Space(VSEEditorUtility.MediumUISpacer);
        GUILayout.Label(
            "The parade spline data hasn't been assigned yet or has been deleted, please assign a spline prefab.",
            VSEEditorUtility.ErrorLabelStyle);

        GUILayout.Space(VSEEditorUtility.LargeUISpacer);

        EditorGUI.BeginChangeCheck();
        currentSplinePrefab = (GameObject)EditorGUILayout.ObjectField(
            ParadeManagerEditor.ParadeSplineGUIContent,
            currentSplinePrefab,
            typeof(GameObject),
            true);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(
                ParadeManager.ParadeSplineObjectPathEditorPref,
                AssetDatabase.GetAssetPath(currentSplinePrefab));
        }

        GUILayout.Space(VSEEditorUtility.SmallUISpacer);

        EditorGUI.BeginDisabledGroup(currentSplinePrefab == null);
        if (GUILayout.Button("Setup Parade"))
        {
            ParadeManagerEditor.SetupParadeSystem();
            Close();
        }
        EditorGUI.EndDisabledGroup();
    }
}
