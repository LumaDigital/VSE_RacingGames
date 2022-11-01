using UnityEngine;
using UnityEngine.Animations;

namespace CameraData
{
    public enum CameraShotType
    {
        Follow,
        LookAt
    }

    [RequireComponent(typeof(Camera), typeof(ParentConstraint))]
    public class RacingGameCamera : MonoBehaviour
    {
        public CameraShotType ShotType;
        public Transform TargetTransform;

        public float FinalFov = 8;
        public float AnimatedFOVStart;
        public float AnimatedFOVEnd;

        private Camera cameraComponent;

        private bool usingFovAnimation;
        private float startFov;
        private float fovTimer;
        private float fovAnimationStartTime;
        private float fovAnimationOffsetEndTime;

        // TODO this shot time will be replaced with the time given by the shot manager
        private float TEMPShotTime = 6.5f;//secs

        public ParentConstraint ParentConstraint
        {
            get
            {
                if (parentConstraint == null)
                    parentConstraint = GetComponent<ParentConstraint>();
                return parentConstraint;
            }
        }
        private ParentConstraint parentConstraint;

        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
            startFov = cameraComponent.fieldOfView;

            if (ShotType == CameraShotType.LookAt && cameraComponent.fieldOfView != FinalFov)
            {
                // Dividing Start/End time by 100 to convert them to a fraction of 1
                fovAnimationStartTime = TEMPShotTime * (AnimatedFOVStart / 100);
                fovAnimationOffsetEndTime = (TEMPShotTime * (AnimatedFOVEnd / 100)) - fovAnimationStartTime;

                usingFovAnimation = true;
            }
        }

        private void Update()
        {
            if (TargetTransform != null)
            {
                if (ShotType == CameraShotType.LookAt)
                {
                    LookAtTarget();

                    if (usingFovAnimation)
                    {
                        fovTimer += Time.deltaTime;
                        if (fovTimer >= fovAnimationStartTime)
                        {
                            float animationTime = fovTimer - fovAnimationStartTime;
                            float animationPercentage = animationTime / fovAnimationOffsetEndTime;
                            cameraComponent.fieldOfView = Mathf.SmoothStep(startFov, FinalFov, animationPercentage);

                            if (animationPercentage >= 1)
                                usingFovAnimation = false;
                        }
                    }
                }
            }
            else
                VSEEditorUtility.LogVSEWarning("No Target Transform for Racing Camera");
        }

        public void LookAtTarget()
        {
            Vector3 lookDirection = TargetTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
