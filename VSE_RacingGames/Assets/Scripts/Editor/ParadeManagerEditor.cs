using UnityEditor;
using UnityEngine;

using CameraData;
using Parade;

[CustomEditor(typeof(ParadeManager))]
public class ParadeManagerEditor : Editor
{
    private const string ParadeObjectName = "Parade";
    private const string CameraObjectName = "Parade Camera";

    private const int ParadeDistanceFromRace = 100;
    private const int ParadeCameraYRotation = 180;
    private const int ParadeCameraZOffset = 1;
    private const float ParadeCameraYOffset = 0.3f;

    private ParadeTypes previousParadeType;

    private SerializedProperty animationClipsProp;
    private RuntimeAnimatorController animatorController;

    public static GUIContent ParadeSplineGUIContent
    {
        get
        {
            return new GUIContent(
                "Parade Spline Prefab",
                "Creating a spline from script causes orientation warnings and bugs, " +
                    "therefore a prefab is required.");
        }
    }

    private ParadeManager ParadeManager
    {
        get
        {
            if (paradeManager == null)
                paradeManager = (ParadeManager)target;
            return paradeManager;
        }
    }
    private ParadeManager paradeManager;

    [MenuItem("VSE/Setup Parade")]
    public static void SetupParadeSystem()
    {
        GameObject paradeObject = GameObject.Find(ParadeObjectName);
        if (paradeObject == null)
        {
            string paradeSplinePath = EditorPrefs.GetString(ParadeManager.ParadeSplineObjectPathEditorPref);
            GameObject paradeSplinePrefab = 
                (GameObject)AssetDatabase.LoadAssetAtPath(paradeSplinePath, typeof(GameObject));

            if (paradeSplinePrefab == null)
            {
                EditorWindow.GetWindow<MissingParadeSplineWindow>("VSE Missing Parade Spline Prefab");
                return;
            }

            paradeObject = new GameObject(ParadeObjectName);
            ParadeManager paradeManager = paradeObject.AddComponent<ParadeManager>();
            paradeManager.ParadeSplinePrefab = paradeSplinePrefab;

            GameObject ParadeCameraObject = new GameObject(CameraObjectName);
            ParadeCameraObject.AddComponent<Camera>();
            ParadeCameraObject.transform.parent = paradeObject.transform;
            ParadeCameraObject.transform.rotation = Quaternion.Euler(new Vector3(0, ParadeCameraYRotation, 0));
            ParadeCameraObject.transform.position += new Vector3(0, ParadeCameraYOffset, ParadeCameraZOffset);

            paradeManager.ParadeCamera = ParadeCameraObject.AddComponent<RacingGameCamera>();
            paradeObject.transform.position = new Vector3(ParadeDistanceFromRace, 0, -ParadeDistanceFromRace);
            paradeManager.SetupParadeEntities();
        }
        else
        {
            VSEEditorUtility.LogVSENotification(ParadeObjectName + " exists and is already setup.");
            Selection.activeObject = paradeObject;
        }
    }

    private void OnEnable()
    {
        previousParadeType = ParadeManager.ParadeType;
        Undo.undoRedoPerformed += ParadeTypeUndoCallBack;
        animationClipsProp = serializedObject.FindProperty(nameof(ParadeManager.StationaryAnimationClips));
        HandleAnimatorController();
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= ParadeTypeUndoCallBack;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        ParadeManager.ParadeSplinePrefab = (GameObject)EditorGUILayout.ObjectField(
            ParadeSplineGUIContent,
            ParadeManager.ParadeSplinePrefab,
            typeof(GameObject),
            true);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(
                ParadeManager.ParadeSplineObjectPathEditorPref,
                AssetDatabase.GetAssetPath(ParadeManager.ParadeSplinePrefab));
        }

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(ParadeManager, nameof(ParadeManager.ParadeType));
        previousParadeType = ParadeManager.ParadeType;
        ParadeManager.ParadeType = (ParadeTypes)EditorGUILayout.EnumPopup("Parade Type", ParadeManager.ParadeType);
        if (EditorGUI.EndChangeCheck())
        {
            if (EditorUtility.DisplayDialog(
                "Change Parade Type",
                "Changing the type will delete the current entities.\n\nAre you sure you want to continue?",
                "Continue",
                "Cancel"))
            {
                ParadeManager.SetupParadeEntities();
                ParadeManager.SetupParade(disableRacersAfterFirst: false);
            }
            else
                ParadeManager.ParadeType = previousParadeType;
        }

        if (ParadeManager.ParadeType == ParadeTypes.Stationary)
        {
            EditorGUI.BeginChangeCheck();
            animatorController = 
                (RuntimeAnimatorController)EditorGUILayout.ObjectField(
                    "Parade Animator Controller",
                    animatorController, 
                    typeof(RuntimeAnimatorController),
                    true);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(
                    ParadeManager.StationaryAnimatorControlerPathEditorPref,
                    AssetDatabase.GetAssetPath(animatorController));
                ParadeManager.SetupParadeAnimatorController(overrideCurrent: true);
            }

            EditorGUILayout.PropertyField(animationClipsProp);
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Preview Racer"))
        {
            if (!ParadeManager.ParadeSetToPlay)
                ParadeManager.SetupParade(disableRacersAfterFirst: false);
            else
                ParadeManager.ReturnRacersToRaceTrack();

            ParadeManager.FocusSceneViewOnRacer();
        }
    }

    private void HandleAnimatorController()
    {
        string controllerPath = ParadeManager.GetStationaryAnimatorControllerPath;
        if (controllerPath != string.Empty)
        {
            animatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(
                controllerPath,
                typeof(RuntimeAnimatorController));
        }
    }

    private void ParadeTypeUndoCallBack()
    {
        if (previousParadeType != ParadeManager.ParadeType)
            ParadeManager.SetupParadeEntities();
    }
}
