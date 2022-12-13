using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Animations;

using CameraData;

namespace Parade
{
    public enum ParadeTypes
    {
        Ring,
        Stationary
    }

    public class ParadeManager : MonoBehaviour
    {
        public const string StationaryAnimatorControlerPathEditorPref = "AnimatorControllerPath";
        public const string ParadeSplineObjectPathEditorPref = "ParadeSplinePath";

        private const string RingParadeObjectName = "Ring Entities";
        private const string StationaryParadeObjectName = "Stationary Entities";
        private const string ParadeCameraTargetName = "Parade Camera Target";
        private const string ParadeSplineObjectName = "Parade Spline";
        private const string StationaryClipName = "Overridable_Clip";

        private const float RingParadeSpeed = 10;

        public float ParadeTime = 30;
        public float ParadeShotTime = 5;

        [HideInInspector]
        public GameObject ParadeSplinePrefab;
        [HideInInspector]
        public List<AnimationClip> StationaryAnimationClips = new List<AnimationClip>();
        [HideInInspector]
        public ParadeTypes ParadeType;
        [HideInInspector]
        public SplineContainer ParadeSplineContainer;
        [HideInInspector]
        public RacingGameCamera ParadeCamera;
        [HideInInspector]
        public SplineAnimate CameraTargetSplineAnimate;
        [HideInInspector]
        public bool ParadeSetToPlay;

        private List<SplineAnimate> racerSplineAnimates = new List<SplineAnimate>();

        private ShotManager shotManager;
        private Vector3 originalCameraPosition;
        private Quaternion originalCameraRotation;

        private float paradeTimer;
        private int paradeMultiplier = 1;
        private int stationaryAnimationIndex;
        private int racerIndex;

        public string GetStationaryAnimatorControllerPath
        {
            get
            {
                // As this is an editor function, the systems needs to be setup to not use this during play
                return EditorPrefs.GetString(StationaryAnimatorControlerPathEditorPref);
            }
        }

        public Animator CameraAnimator
        {
            get
            {
                if (cameraAnimator == null)
                {
                    if (ParadeCamera.transform.parent.GetComponent<Animator>() == null)
                        cameraAnimator = ParadeCamera.transform.parent.gameObject.AddComponent<Animator>();
                    else
                        cameraAnimator = ParadeCamera.transform.parent.GetComponent<Animator>();
                }
                return cameraAnimator;
            }
        }
        private Animator cameraAnimator;

        private AnimatorOverrideController OverrideController
        {
            get
            {
                if (overrideController == null)
                {
                    overrideController =
                        new AnimatorOverrideController(CameraAnimator.runtimeAnimatorController);
                }
                return overrideController;
            }
        }
        private AnimatorOverrideController overrideController;

        // OverrideController overrides via clip name, but after overriding a clip, the name won't change
        private string overrideableClipName;

        private RacingGame RacingGameData
        {
            get
            {
                if (racingGameData == null)
                    racingGameData = FindObjectOfType<RacingGame>();
                return racingGameData;
            }
        }
        private RacingGame racingGameData;

        private void Awake()
        {
            shotManager = FindObjectOfType<ShotManager>();
            if (shotManager.RunRaceShots)
            {
                VSEEditorUtility.LogVSEWarning("Shot Manager's 'RunRaceShots' is set to true.\n" +
                    "For the Parade system to work correctly, disable this field.");
            }

            if (ParadeType == ParadeTypes.Stationary)
            {

                if (CameraAnimator.runtimeAnimatorController == null)
                {
                    VSEEditorUtility.LogVSEError("Stationary Parade Animator Controller missing.\n" +
                        "Please add the correct controller to the 'Parade Animator Controller' field.");
                    return;
                }

                CameraAnimator.runtimeAnimatorController = OverrideController;
                if (OverrideController.animationClips.Length <= 0)
                {
                    VSEEditorUtility.LogVSEError("'" + StationaryClipName + "' is missing its animation motion.\n" +
                        "Please assign a motion to the default state of the Parade controller and try again.");
                    return;
                }

                overrideableClipName = OverrideController.animationClips[0].name;
            }

            SetupParade();
            CheckRacerAvailability();
        }

        private void FixedUpdate()
        {
            HandleParade();
        }

        public void SetupParadeEntities()
        {
            if (RacingGameData == null || RacingGameData.CurrentRaceSplineContainer == null)
            {
                VSEEditorUtility.LogVSEError("Racing Game data hasn't been setup correctly.\nPlease setup from VSE > Race Manager.");
                DestroyImmediate(ParadeCamera.transform.parent.gameObject);
                return;
            }

            GameObject entitiesObject;
            bool animateOnSpline = false;
            if (ParadeType == ParadeTypes.Ring)
            {
                entitiesObject = HandleEntityParents(StationaryParadeObjectName, RingParadeObjectName);
                ParadeCamera.ShotType = CameraShotType.Follow;
                animateOnSpline = true;
            }
            else
            {
                entitiesObject = HandleEntityParents(RingParadeObjectName, StationaryParadeObjectName);
                ParadeCamera.ShotType = CameraShotType.LookAt;
            }

            // Stationary parades also use a spline as unity logs an error if the spline is removed from the SplineAnimate.
            // Therefore the same spline is used, but the racers will not run it if set to stationary parade.

            GameObject splineObject = Instantiate(ParadeSplinePrefab, entitiesObject.transform.position, Quaternion.identity);
            splineObject.transform.parent = entitiesObject.transform;
            splineObject.name = ParadeSplineObjectName;

            ParadeSplineContainer = splineObject.GetComponent<SplineContainer>();

            Transform cameraTarget = new GameObject(ParadeCameraTargetName).transform;
            cameraTarget.position = ParadeSplineContainer.transform.position;
            cameraTarget.transform.parent = entitiesObject.transform;
            VSEUtility.OrientTransformToSpline(cameraTarget, ParadeSplineContainer);

            ParadeCamera.TargetTransform = cameraTarget;

            CameraTargetSplineAnimate = cameraTarget.gameObject.AddComponent<SplineAnimate>();
            CameraTargetSplineAnimate.splineContainer = ParadeSplineContainer;
            CameraTargetSplineAnimate.method = SplineAnimate.Method.Speed;
            CameraTargetSplineAnimate.maxSpeed = RingParadeSpeed;
            CameraTargetSplineAnimate.playOnAwake = animateOnSpline;

            ParadeCamera.EnableTargetParentConstraints();
        }

        public void SetupParade(bool disableRacersAfterFirst = true)
        {
            if (ParadeShotTime > 0)
            {
                ParadeSetToPlay = true;

                AssignRacerSplineAnimates();
                bool enableFirstRacer = true;
                foreach (SplineAnimate racerSplineAnimate in racerSplineAnimates)
                {
                    if (disableRacersAfterFirst)
                    {
                        racerSplineAnimate.gameObject.SetActive(enableFirstRacer);
                        if (enableFirstRacer)
                        {
                            HandleRacerParentConstraint(racerSplineAnimate.transform);
                            enableFirstRacer = false;
                        }
                    }

                    racerSplineAnimate.splineContainer = ParadeSplineContainer;
                    if (ParadeType == ParadeTypes.Ring)
                    {
                        racerSplineAnimate.playOnAwake = true;

                        if (Application.isPlaying)
                            racerSplineAnimate.Play();
                    }
                    else
                    {
                        ParadeCamera.LookAtTarget();
                        SaveCameraTransforms();
                        racerSplineAnimate.transform.position = transform.position;
                        racerSplineAnimate.playOnAwake = false;
                    }
                    VSEUtility.OrientTransformToSpline(racerSplineAnimate.transform, ParadeSplineContainer);
                }

                if (ParadeType == ParadeTypes.Stationary)
                {
                    SetupParadeAnimatorController();
                    HandleStationaryCameraAnimation();
                }

                ParadeCamera.GetComponent<ParentConstraint>().enabled = ParadeType == ParadeTypes.Ring;
            }
        }

        public void ReturnRacersToRaceTrack()
        {
            SplineContainer currentRaceSplineContainer = RacingGameData.CurrentRaceSplineContainer;
            foreach (SplineAnimate racerSplineAnimate in racerSplineAnimates)
            {
                racerSplineAnimate.elapsedTime = 0;
                racerSplineAnimate.gameObject.SetActive(true);
                racerSplineAnimate.splineContainer = currentRaceSplineContainer;

                VSEUtility.OrientTransformToSpline(racerSplineAnimate.transform, currentRaceSplineContainer);

                if (Application.isPlaying)
                    racerSplineAnimate.Play();
            }

            ParadeSetToPlay = false;
        }

        public void FocusSceneViewOnRacer()
        {
            if (racerSplineAnimates.Count <= 0)
                AssignRacerSplineAnimates();

            Transform racerTransform = racerSplineAnimates[0].transform;
            Selection.activeTransform = racerTransform;
            SceneView.FrameLastActiveSceneView();
            Selection.activeTransform = transform;
        }

        public void AssignRacerSplineAnimates()
        {
            // TODO: Will probably have to order the racers in their race order
            racerSplineAnimates = FindObjectsOfType<SplineAnimate>().ToList();
            racerSplineAnimates = racerSplineAnimates.Where(item => item.name != ParadeCameraTargetName).ToList();
        }

        public void SetupParadeAnimatorController(bool overrideCurrent = false)
        {
            if (overrideCurrent || CameraAnimator.runtimeAnimatorController == null)
            {
                CameraAnimator.runtimeAnimatorController =
                    (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(
                        GetStationaryAnimatorControllerPath,
                        typeof(RuntimeAnimatorController));
            }
        }

        private void CheckRacerAvailability()
        {
            if (RacingGameData.NumberOfRacers > racerSplineAnimates.Count)
            {
                VSEEditorUtility.LogVSEError(
                    "Less racers with SplineAnimates than the selected number of racers: " + RacingGameData.NumberOfRacers +
                    "\nThe system still needs to spawn racers based on race game data number.");
            }
        }

        private void HandleParade()
        {
            if (ParadeSetToPlay)
            {
                paradeTimer += Time.deltaTime;
                if (paradeTimer > ParadeTime)
                {
                    gameObject.SetActive(false);
                    ReturnRacersToRaceTrack();
                    shotManager.RunRaceShots = true;

                    return;
                }

                if (paradeTimer > ParadeShotTime * paradeMultiplier)
                {
                    racerSplineAnimates[racerIndex].gameObject.SetActive(false);
                    racerIndex++;
                    if (racerIndex + 1 > RacingGameData.NumberOfRacers)
                        racerIndex = 0;

                    if (ParadeType == ParadeTypes.Stationary)
                    {
                        RevertCameraTransforms();
                        HandleStationaryCameraAnimation();
                    }

                    HandleRacerParentConstraint(racerSplineAnimates[racerIndex].transform);
                    racerSplineAnimates[racerIndex].gameObject.SetActive(true);
                    racerSplineAnimates[racerIndex].elapsedTime = 0;
                    CameraTargetSplineAnimate.elapsedTime = 0;
                    paradeMultiplier++;
                }
            }
        }

        private void HandleStationaryCameraAnimation()
        {
            if (StationaryAnimationClips.Count > 0)
            {
                OverrideController[overrideableClipName] = StationaryAnimationClips[stationaryAnimationIndex];
                CameraAnimator.Play(StationaryClipName, layer: 0, normalizedTime: 0);

                stationaryAnimationIndex++;
                if (stationaryAnimationIndex >= StationaryAnimationClips.Count)
                    stationaryAnimationIndex = 0;
            }
        }

        private GameObject HandleEntityParents(string oldEntitiesObjectName, string newEntitiesObjectName)
        {
            GameObject oldEntitiesObject = GameObject.Find(oldEntitiesObjectName);
            if (oldEntitiesObject != null)
                DestroyImmediate(oldEntitiesObject);

            GameObject newEntitiesObject = new GameObject(newEntitiesObjectName);
            newEntitiesObject.transform.position = transform.position;
            newEntitiesObject.transform.parent = transform;

            return newEntitiesObject;
        }

        private void SaveCameraTransforms()
        {
            originalCameraPosition = ParadeCamera.transform.position;
            originalCameraRotation = ParadeCamera.transform.rotation;
        }

        private void RevertCameraTransforms()
        {
            ParadeCamera.transform.position = originalCameraPosition;
            ParadeCamera.transform.rotation = originalCameraRotation;
        }

        private void HandleRacerParentConstraint(Transform racerTransform)
        {
            if (ParadeType == ParadeTypes.Ring)
            {
                ParadeCamera.TargetTransform = racerTransform;
                ParadeCamera.EnableTargetParentConstraints();
            }
        }
    }
}

