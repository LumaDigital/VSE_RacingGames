using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShotManager))]
public class ShotManagerEditor : Editor
{
    private ShotManager ShotManager;
    private int numberOfShots;

    private void OnEnable()
    {
        ShotManager = (ShotManager)target;
    }

    public override void OnInspectorGUI()
    {   
        EditorGUI.BeginChangeCheck();
        numberOfShots = EditorGUILayout.IntField("Number of Shots:", ShotManager.ListOfShots.Count);
        if (EditorGUI.EndChangeCheck())
        {
            if (numberOfShots > Utility.Alphabet.Length)
                numberOfShots = Utility.Alphabet.Length;

            Undo.RecordObject(ShotManager, nameof(ShotManager.ListOfShots));
            int countDifference = Mathf.Abs(numberOfShots - ShotManager.ListOfShots.Count);
            if (numberOfShots > ShotManager.ListOfShots.Count)
            {
                for (int i = 0; i < countDifference; i++)
                    ShotManager.ListOfShots.Add(new ShotData());
            }
            else if (numberOfShots >= 0)
            {
                ShotManager.ListOfShots.RemoveRange(numberOfShots, countDifference);
            }

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        ShotManager.Length = ShotManager.SplineContainer.CalculateLength();

        if (numberOfShots > 0)
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

                EditorGUILayout.BeginHorizontal();  
                GUILayout.Space(15);
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
        if (shotIndex >= 1 && shotIndex <= ShotManager.ListOfShots.Count)
        {
            if (ShotManager.ListOfShots[shotIndex].TriggerPosition < ShotManager.ListOfShots[shotIndex - 1].TriggerPosition)
                ShotManager.ListOfShots[shotIndex].TriggerPosition = ShotManager.ListOfShots[shotIndex - 1].TriggerPosition;
        }
    }
}
