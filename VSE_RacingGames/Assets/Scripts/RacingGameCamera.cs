using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Splines;

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
        public bool FocusCameraAfterEditorMove;
        public CameraShotType ShotType;
        public Transform TargetTransform;

        private ConstraintSource constraintSource = new ConstraintSource() { weight = 1 };
        public ShotData ShotData;

        public ShotManager ShotManager
        {
            get
            {
                if (shotManager == null)
                {
                    SplineContainer activeSpline = GameObject.Find("Track_Splines").transform.GetComponentInChildren<SplineContainer>();
                    shotManager = GameObject.Find(activeSpline.name).GetComponent<ShotManager>();
                }
                return shotManager;
            }
        }
        private ShotManager shotManager;

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

        private void Update()
        {
            if (TargetTransform != null)
            {
                if (ShotType == CameraShotType.LookAt)
                    LookAtTarget();
            }
            else
                VSEEditorUtility.LogVSEWarning("No Target Transform for Racing Camera");
        }

        public void LookAtTarget()
        {
            Vector3 lookDirection = TargetTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        public void EnableTargetParentConstraints()
        {
            if (TargetTransform != null)
            {
                constraintSource.sourceTransform = TargetTransform;

                if (ParentConstraint.sourceCount == 0) // new
                {
                    ParentConstraint.constraintActive = true;
                    ParentConstraint.AddSource(constraintSource);
                }
                else
                    ParentConstraint.SetSource(index: 0, source: constraintSource);

            }
        }
    }
}
