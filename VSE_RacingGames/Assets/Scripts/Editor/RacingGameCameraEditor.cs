using UnityEditor;

using UnityEngine;
using UnityEngine.Animations;

using CameraData;

[CustomEditor(typeof(RacingGameCamera))]
public class RacingGameCameraEditor : Editor
{
    private RacingGameCamera racingGameCamera;

    private void OnEnable()
    {
        racingGameCamera = (RacingGameCamera)target;
    }

    public override void OnInspectorGUI()
    {
        if (!Application.isPlaying && racingGameCamera.ParentConstraint.locked)
        {
            VSEEditorUtility.LogVSEWarning("Parent Constraint 'lock activate' permission denied." +
                "\nRacing cameras can't have their constraints locked.");

            racingGameCamera.ParentConstraint.locked = false;
        }

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.ShotType));
        racingGameCamera.ShotType = (CameraShotType)EditorGUILayout.EnumPopup("Shot Type", racingGameCamera.ShotType);
        if (EditorGUI.EndChangeCheck())
        {
            if (racingGameCamera.ShotType == CameraShotType.Follow)
                racingGameCamera.EnableTargetParentConstraints();
            else
                DisableTargetParentConstraints();
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.TargetTransform));
        racingGameCamera.TargetTransform =
            (Transform)EditorGUILayout.ObjectField("Target Transform", racingGameCamera.TargetTransform, typeof(Transform), true);
        if (EditorGUI.EndChangeCheck() && racingGameCamera.ShotType == CameraShotType.Follow)
            racingGameCamera.EnableTargetParentConstraints();

        EditorGUI.BeginDisabledGroup(racingGameCamera.TargetTransform == null);
        Undo.RecordObject(racingGameCamera.transform, nameof(racingGameCamera.transform.rotation));
        if (GUILayout.Button("Focus"))
            racingGameCamera.LookAtTarget();
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        racingGameCamera.FocusCameraAfterEditorMove =
            EditorGUILayout.Toggle("Focuse Camera After Editor Move", racingGameCamera.FocusCameraAfterEditorMove);

        if (racingGameCamera.FocusCameraAfterEditorMove && racingGameCamera.transform.hasChanged)
        {
            racingGameCamera.transform.hasChanged = false;
            racingGameCamera.LookAtTarget();
        }
    }

    private void DisableTargetParentConstraints()
    {
        if (racingGameCamera.ParentConstraint.sourceCount > 0)
        {
            racingGameCamera.ParentConstraint.RemoveSource(index: 0);
            racingGameCamera.ParentConstraint.constraintActive = false;
        }
    }
}
