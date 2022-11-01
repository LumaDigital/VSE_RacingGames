using UnityEditor;

using UnityEngine;
using UnityEngine.Animations;

using CameraData;

[CustomEditor(typeof(RacingGameCamera))]
public class RacingGameCameraEditor : Editor
{
    private const float MinimumObstructionFovDifference = 10;
    private const int MaximumDurationLabelPadding = -15;
    private const int MinimumDurationLabelPadding = -10;

    private RacingGameCamera racingGameCamera;
    private ConstraintSource constraintSource = new ConstraintSource() { weight = 1 };

    private GUIContent FovAnimationControlContent
    {
        get
        {
            if (fovAnimationControlContent == null)
            {
                fovAnimationControlContent = new GUIContent();
                fovAnimationControlContent.text = "FOV Animation";
                fovAnimationControlContent.tooltip = 
                    "This slider is used to setup the FOV to animate from its current value on 'Play' to the " +
                    "'Final FOV' of this component.\n\nThe animation duration is determined by the bar's size, " +
                    "where the slider's length represents the shot's duration.";
            }
            return fovAnimationControlContent;
        }
    }
    private GUIContent fovAnimationControlContent;

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

        if (racingGameCamera.ShotType == CameraShotType.LookAt)
        {
            Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.FinalFov));
            racingGameCamera.FinalFov = EditorGUILayout.FloatField("Final FOV", racingGameCamera.FinalFov);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(FovAnimationControlContent);
            EditorGUILayout.Space();

            racingGameCamera.AnimatedFOVStart = 
                EditorGUILayout.FloatField(racingGameCamera.AnimatedFOVStart, VSEEditorUtility.ThreeDigitWidthLayoutOption);
            VSEEditorUtility.RoundOffToOneDecimals(ref racingGameCamera.AnimatedFOVStart);

            Undo.RecordObject(racingGameCamera, nameof(racingGameCamera.AnimatedFOVStart) + "_" + nameof(racingGameCamera.AnimatedFOVEnd));
            EditorGUILayout.MinMaxSlider(
                ref racingGameCamera.AnimatedFOVStart,
                ref racingGameCamera.AnimatedFOVEnd,
                0,
                100);

            float fovStartEndDifference = racingGameCamera.AnimatedFOVEnd - racingGameCamera.AnimatedFOVStart;
            fovStartEndDifference = (Mathf.Round(fovStartEndDifference * 10)) / 10;

            float middleOfSliderPercentage = (racingGameCamera.AnimatedFOVStart + (fovStartEndDifference / 2)) / 100;

            Rect sliderRectPosition = GUILayoutUtility.GetLastRect();
            float yObstructionOffset = fovStartEndDifference < MinimumObstructionFovDifference ? sliderRectPosition.height / 2 : 0;

            // Weird Unity behaviour: sliderRectPosition.x/y is zero, but hardcoding 0 as a argument results in the -
            // control moving to the beginning of the inspector's width
            sliderRectPosition = new Rect(
                sliderRectPosition.x + (sliderRectPosition.width * middleOfSliderPercentage),
                sliderRectPosition.y + yObstructionOffset,
                sliderRectPosition.width,
                sliderRectPosition.height);

            racingGameCamera.AnimatedFOVEnd = 
                EditorGUILayout.FloatField(racingGameCamera.AnimatedFOVEnd, VSEEditorUtility.ThreeDigitWidthLayoutOption);
            racingGameCamera.AnimatedFOVEnd = (Mathf.Round(racingGameCamera.AnimatedFOVEnd * 10)) / 10;

            EditorGUILayout.EndHorizontal();

            // Account for label going off center as its drawn from its left anchor
            GUIStyle offsetStyle = new GUIStyle();
            offsetStyle.alignment = TextAnchor.MiddleLeft;
            offsetStyle.padding.left = yObstructionOffset == 0 ? MaximumDurationLabelPadding : MinimumDurationLabelPadding;
            offsetStyle.normal.textColor = Color.white;

            GUI.Label(sliderRectPosition, fovStartEndDifference.ToString() + "%", offsetStyle);
        }
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
