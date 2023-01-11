using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

using CameraData;

public class VSEScriptMenu
{
    [MenuItem("VSE/Add Components/Spline Tools/Shot Manager")]
    public static void AddShotManagerComponent()
    {
        AddComponentToSelectedObject(typeof(ShotManager), typeof(SplineContainer));
    }

    [MenuItem("VSE/Add Components/Spline Tools/Spline Gizmo Display")]
    public static void AddSplineDisplayComponent()
    {
        AddComponentToSelectedObject(typeof(SplineDisplay), typeof(SplineContainer));
    }

    [MenuItem("VSE/Add Components/Spline Tools/Spline Length Display")]
    public static void AddSplineLengthComponent()
    {
        AddComponentToSelectedObject(typeof(SplineLength), typeof(SplineContainer));
    }

    [MenuItem("VSE/Add Components/Racer's Progress Display")]
    public static void AddRacerDistanceComponent()
    {
        AddComponentToSelectedObject(typeof(RacerDistance), typeof(SplineAnimate));
    }
    
    [MenuItem("VSE/Add Components/Racing Game Camera")]
    public static void AddRacingGameCameraComponent()
    {
        AddComponentToSelectedObject(typeof(RacingGameCamera), typeof(Camera));
    }

    static void AddComponentToSelectedObject(Type componentToAdd, Type requiredComponent = null)
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("No object selected!", "Please select an object before adding Tool!", "Okay");
            return;
        }

        if (requiredComponent != null && Selection.activeGameObject.GetComponent(requiredComponent) == null)
        {
            if (DisplayMissingComponentDialog(requiredComponent))
                Selection.activeGameObject.AddComponent(requiredComponent);
            else
                return;
        }

        Selection.activeGameObject.AddComponent(componentToAdd);
    }

    static bool DisplayMissingComponentDialog(Type missingComponent)
    {
        return EditorUtility.DisplayDialog("Failed to add Tool to selected object!", "Tool requires the selected object " +
            "to contain necessary component: " + missingComponent, "Add Missing Component", "Cancel");
    }
}