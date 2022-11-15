using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShotManager))]
public class ShotManagerEditor : Editor
{
    private const float LookAtValue = 0.01f;

    private ShotManager shotManager;
    private int previousNumberOfShots;

    private void OnEnable()
    {
        shotManager = (ShotManager)target;
        shotManager.NumberOfShots = shotManager.ListOfShots.Count;

        Undo.undoRedoPerformed += UndoCallBack;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(shotManager, nameof(shotManager.NumberOfShots));
        shotManager.NumberOfShots = EditorGUILayout.IntSlider("Number of Shots:",
            shotManager.NumberOfShots,
            leftValue: 0,
            rightValue: VSEUtility.Alphabet.Length);

        // Prevents manually entering values
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            GUI.FocusControl(null);

        if (shotManager.NumberOfShots > VSEUtility.Alphabet.Length)
            shotManager.NumberOfShots = VSEUtility.Alphabet.Length;
        else if (shotManager.NumberOfShots < 0)
            shotManager.NumberOfShots = 0;

        if (previousNumberOfShots != shotManager.NumberOfShots)
            previousNumberOfShots = shotManager.NumberOfShots;

        if (shotManager.NumberOfShots != shotManager.ListOfShots.Count)
        {
            int countDifference = Mathf.Abs(shotManager.NumberOfShots - shotManager.ListOfShots.Count);
            if (shotManager.NumberOfShots > shotManager.ListOfShots.Count)
            {
                for (int i = 0; i < countDifference; i++)
                {
                    ShotData newShot = new ShotData();
                    if (shotManager.ListOfShots.Count > 0 && shotManager.ListOfShots.Count < VSEUtility.Alphabet.Length)
                        newShot.TriggerObject = new GameObject(VSEUtility.Alphabet[shotManager.ListOfShots.Count].ToString());
                    else
                        newShot.TriggerObject = new GameObject(VSEUtility.Alphabet[i].ToString());

                    newShot.TriggerObject.transform.parent = shotManager.transform;
                    newShot.TriggerObject.AddComponent(typeof(ShotTrigger));
                    newShot.TriggerObject.SetActive(shotManager.ToggleShotTriggerDisplay);
                    shotManager.ListOfShots.Add(newShot);

                    UpdateTriggerPosition(newShot);
                }
            }
            else
            {
                for (int i = shotManager.ListOfShots.Count - 1; i >= shotManager.ListOfShots.Count - countDifference; i--)
                    DestroyImmediate(shotManager.ListOfShots[i].TriggerObject);

                shotManager.ListOfShots.RemoveRange(shotManager.NumberOfShots, countDifference);
            }
        }

        if (shotManager.NumberOfShots > 0)
            ShowShots();
    }

    private void ShowShots()
    {
        EditorGUI.BeginChangeCheck();
        shotManager.ToggleShotTriggerDisplay = EditorGUILayout.Toggle(
            "Toggle Shot Trigger Display:",
            shotManager.ToggleShotTriggerDisplay);
        if (EditorGUI.EndChangeCheck())
        { 
            if (shotManager.ToggleShotTriggerDisplay)
            {
                foreach (ShotData shot in shotManager.ListOfShots)
                {
                    Undo.RecordObject(shot.TriggerObject, nameof(shot.TriggerObject));
                    shot.TriggerObject.SetActive(shotManager.ToggleShotTriggerDisplay);
                }
            }
            else if (!shotManager.ToggleShotTriggerDisplay)
            {
                foreach (ShotData shot in shotManager.ListOfShots)
                {
                    Undo.RecordObject(shot.TriggerObject, nameof(shot.TriggerObject));
                    shot.TriggerObject.SetActive(shotManager.ToggleShotTriggerDisplay);
                }
            }

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        foreach (ShotData shot in shotManager.ListOfShots)
        {
            int shotIndex = shotManager.ListOfShots.IndexOf(shot);
            string shotName = "Shot " + (shotIndex + 1);

            GUILayout.BeginHorizontal("box");
            shot.ToggleShotFoldout = EditorGUILayout.Foldout(shot.ToggleShotFoldout, shotName);
            GUILayout.EndHorizontal();

            if (shot.ToggleShotFoldout)
            {
                shot.CameraComponent = (Camera)EditorGUILayout.ObjectField("Camera Object:",
                    shot.CameraComponent,
                    typeof(Camera),
                    allowSceneObjects: true);

                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(shotManager, nameof(shot.TriggerPosition));
                shot.TriggerPosition = EditorGUILayout.Slider(
                    "Trigger Distance " + VSEUtility.Alphabet[shotIndex] + ":",
                    shot.TriggerPosition,
                    leftValue: 0f,
                    shotManager.SplineLength.Length);

                if (EditorGUI.EndChangeCheck())
                {
                    UpdateTriggerPosition(shot);
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(VSEEditorUtility.MediumUISpacer);
                shot.ToggleEntityFoldout = EditorGUILayout.Foldout(shot.ToggleEntityFoldout,
                    "Number of Entities:");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                int numberOfProps = EditorGUILayout.IntField("\t", shot.ListOfEntities.Count);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    int countDifference = Mathf.Abs(numberOfProps - shot.ListOfEntities.Count);
                    if (numberOfProps > shot.ListOfEntities.Count)
                    {
                        for (int i = 0; i < countDifference; i++)
                            shot.ListOfEntities.Add(null);
                    }
                    else if (numberOfProps < shot.ListOfEntities.Count && numberOfProps >= 0)
                        shot.ListOfEntities.RemoveRange(numberOfProps, countDifference);
                }

                if (shot.ToggleEntityFoldout)
                {
                    for (int i = 0; i < shot.ListOfEntities.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(i + ": ");
                        shot.ListOfEntities[i] = (GameObject)EditorGUILayout.ObjectField(
                            shot.ListOfEntities[i],
                            typeof(GameObject),
                            allowSceneObjects: true);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            UpdateNextTriggerToPreviousTrigger(shotIndex);
        }
    }

    private void UpdateNextTriggerToPreviousTrigger(int shotIndex)
    {
        if (shotIndex >= 1 && shotIndex <= shotManager.ListOfShots.Count)
        {
            if (shotManager.ListOfShots[shotIndex].TriggerPosition < shotManager.ListOfShots[shotIndex - 1].TriggerPosition)
            {
                shotManager.ListOfShots[shotIndex].TriggerPosition = shotManager.ListOfShots[shotIndex - 1].TriggerPosition;
                UpdateTriggerPosition(shotManager.ListOfShots[shotIndex]);
            }
        }
    }

    private void UpdateTriggerPosition(ShotData shot)
    {
        shot.TriggerObject.transform.position = shotManager.SplineContainer.EvaluatePosition(
            shot.TriggerPosition / shotManager.SplineContainer.CalculateLength());
        shot.TriggerObject.transform.LookAt(shotManager.SplineContainer.EvaluatePosition(
            shot.TriggerPosition / shotManager.SplineContainer.CalculateLength() + LookAtValue));
    }

    void UndoCallBack()
    {
        if (shotManager.NumberOfShots > previousNumberOfShots)
        {
            foreach (ShotData shot in shotManager.ListOfShots)
            {
                if (shot.TriggerObject == null)
                {
                    shot.TriggerObject = new GameObject(VSEUtility.Alphabet[shotManager.ListOfShots.IndexOf(shot)].ToString());
                    shot.TriggerObject.transform.parent = shotManager.transform;
                    shot.TriggerObject.AddComponent(typeof(ShotTrigger));
                    shot.TriggerObject.SetActive(shotManager.ToggleShotTriggerDisplay);

                    UpdateTriggerPosition(shot);
                }
            }
        }
        else if (shotManager.NumberOfShots < previousNumberOfShots)
        {
            for (var i = shotManager.transform.childCount - 1; i >= shotManager.NumberOfShots; i--)
                DestroyImmediate(shotManager.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < shotManager.ListOfShots.Count; i++)
        {
            if (shotManager.ListOfShots[i].TriggerObject != null)
                UpdateTriggerPosition(shotManager.ListOfShots[i]);
        }

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
}
