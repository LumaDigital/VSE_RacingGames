using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShotManager))]
public class ShotManagerEditor : Editor
{
    private ShotManager ShotManager;
    int previousNumberOfShots;

    private void OnEnable()
    {
        ShotManager = (ShotManager)target;
        ShotManager.Length = ShotManager.SplineContainer.CalculateLength();

        ShotManager.NumberOfShots = ShotManager.ListOfShots.Count;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(ShotManager, nameof(ShotManager.NumberOfShots));
        ShotManager.NumberOfShots = EditorGUILayout.IntField("Number of Shots:", ShotManager.NumberOfShots);

        if (ShotManager.NumberOfShots != previousNumberOfShots)
        {
            Debug.Log("Hi there");
            // Gotta move shot stuff here
        }
        previousNumberOfShots = ShotManager.NumberOfShots;

        if (ShotManager.NumberOfShots != ShotManager.ListOfShots.Count)// Actually check if change check can be used instead. Needs to be working with redo/undo
        {
            if (ShotManager.NumberOfShots > Utility.Alphabet.Length)
                ShotManager.NumberOfShots = Utility.Alphabet.Length;

            int countDifference = Mathf.Abs(ShotManager.NumberOfShots - ShotManager.ListOfShots.Count);
            if (ShotManager.NumberOfShots > ShotManager.ListOfShots.Count)//increment
            {
                for (int i = 0; i < countDifference; i++)
                {
                    ShotData newShot = new ShotData();
                    newShot.TriggerObject = new GameObject(Utility.Alphabet[i].ToString());
                    newShot.TriggerObject.transform.parent = ShotManager.transform;
                    newShot.TriggerObject.AddComponent(typeof(ShotTrigger));

                    ShotManager.ListOfShots.Add(newShot);

                    // Call method that rotates and positions the argument shot
                }
            }
            else//decrement
            {
                for (int i = ShotManager.ListOfShots.Count - 1; i >= ShotManager.ListOfShots.Count - countDifference; i--)
                {
                    DestroyImmediate(ShotManager.ListOfShots[i].TriggerObject);
                }

                ShotManager.ListOfShots.RemoveRange(ShotManager.NumberOfShots, countDifference);
            }
        }

        if (ShotManager.NumberOfShots > 0)
            ShowShots();
    }

    private void ShowShots()
    {
        EditorGUI.BeginChangeCheck();
        ShotManager.ToggleShotTriggerDisplay = EditorGUILayout.Toggle(
            "Toggle Shot Trigger Display:",
            ShotManager.ToggleShotTriggerDisplay);
        if (EditorGUI.EndChangeCheck())
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

        foreach (var shot in ShotManager.ListOfShots)
        {
            int shotIndex = ShotManager.ListOfShots.IndexOf(shot);
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
                shot.TriggerPosition = EditorGUILayout.IntSlider(
                    "Trigger " + Utility.Alphabet[shotIndex] + ":",
                    shot.TriggerPosition,
                    leftValue: 0,
                    (int)ShotManager.Length);
                if (EditorGUI.EndChangeCheck())
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    // Call method that rotates and positions the argument shot

                EditorGUILayout.BeginHorizontal();  
                GUILayout.Space(15);
                shot.ToggleEntityFoldout = EditorGUILayout.Foldout(shot.ToggleEntityFoldout,
                    "Number of Entities:");
                GUILayout.FlexibleSpace();// Just check if you can use just one of these calls
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
                        GUILayout.FlexibleSpace();//// Just check if you can use just one of these calls
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
        if (shotIndex >= 1 && shotIndex <= ShotManager.ListOfShots.Count)
        {
            if (ShotManager.ListOfShots[shotIndex].TriggerPosition < ShotManager.ListOfShots[shotIndex - 1].TriggerPosition)
                ShotManager.ListOfShots[shotIndex].TriggerPosition = ShotManager.ListOfShots[shotIndex - 1].TriggerPosition;
        }
    }
}
