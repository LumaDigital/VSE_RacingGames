using UnityEditor;

using UnityEngine;
using UnityEngine.Animations;

using CameraData;

[CustomEditor(typeof(RacingGameCamera))]
public class RacingGameCameraEditor : Editor
{
    private RacingGameCamera racingGameCamera;
    private ConstraintSource constraintSource = new ConstraintSource() { weight = 1 };


    private void OnEnable()
    {
        racingGameCamera = (RacingGameCamera)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.ShotType));
        racingGameCamera.ShotType = (CameraShotType)EditorGUILayout.EnumPopup("Shot Type", racingGameCamera.ShotType);
        if (EditorGUI.EndChangeCheck())
        {
            if (racingGameCamera.ShotType == CameraShotType.Follow)
                EnableTargetParentConstraints();
            else
                DisableTargetParentConstraints();
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.TargetTransform));
        racingGameCamera.TargetTransform =
            (Transform)EditorGUILayout.ObjectField("Target Transform", racingGameCamera.TargetTransform, typeof(Transform), true);
        if (EditorGUI.EndChangeCheck() && racingGameCamera.ShotType == CameraShotType.Follow)
            EnableTargetParentConstraints();

        EditorGUI.BeginDisabledGroup(racingGameCamera.TargetTransform == null);
        Undo.RecordObject(racingGameCamera.transform, nameof(racingGameCamera.transform.rotation));
        if (GUILayout.Button("Focus"))
            racingGameCamera.LookAtTarget();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }
    private void EnableTargetParentConstraints()
    {
        if (racingGameCamera.TargetTransform != null)
        {
            if (racingGameCamera.ParentConstraint.sourceCount == 0) // new
                racingGameCamera.ParentConstraint.constraintActive = true;
            else if (racingGameCamera.TargetTransform != constraintSource.sourceTransform) // reset
                racingGameCamera.ParentConstraint.RemoveSource(index: 0);
            else // same selection
                return;

            constraintSource.sourceTransform = racingGameCamera.TargetTransform;
            racingGameCamera.ParentConstraint.AddSource(constraintSource);
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
