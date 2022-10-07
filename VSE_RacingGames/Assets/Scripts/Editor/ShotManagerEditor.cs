using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShotManager))]
public class ShotManagerEditor : Editor
{
    private ShotManager shotManager;
    private int currentNumberOfShots;

    private void OnEnable()
    {
        shotManager = (ShotManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        currentNumberOfShots = EditorGUILayout.IntField("Number of Shots:", shotManager.ShotsList.Count);
        if (EditorGUI.EndChangeCheck())
        {
            int maxAlphabetLengthForTriggerPairs = Utility.Alphabet.Length - 1;
            if (currentNumberOfShots > maxAlphabetLengthForTriggerPairs)
                currentNumberOfShots = maxAlphabetLengthForTriggerPairs;

            int countDifference = Mathf.Abs(currentNumberOfShots - shotManager.ShotsList.Count);
            if (currentNumberOfShots > shotManager.ShotsList.Count)
            {
                for (int i = 0; i < countDifference; i++)
                    shotManager.ShotsList.Add(new ShotData());
            }
            else if (currentNumberOfShots < shotManager.ShotsList.Count)
            {
                if (currentNumberOfShots <= 0)
                    shotManager.ShotsList = new List<ShotData>();
                else
                    shotManager.ShotsList.RemoveRange((shotManager.ShotsList.Count - 1) - countDifference, countDifference);
            }
        }

        if (currentNumberOfShots > 0)
            DisplayShotData();
    }

    private void DisplayShotData()
    {
        foreach (ShotData shot in shotManager.ShotsList)
        {
            int shotIndex = shotManager.ShotsList.IndexOf(shot);

            string shotName = "Shot " + (shotIndex + 1) + ": ";
            if (shot.ShotParentGameObject != null)
                shotName += shot.ShotParentGameObject.name;

            GUILayout.BeginHorizontal("box");
            shot.ShowDataControls = EditorGUILayout.Foldout(shot.ShowDataControls, shotName);
            GUILayout.EndHorizontal();
            if (shot.ShowDataControls)
            {
                shot.ShotParentGameObject = (GameObject)EditorGUILayout.ObjectField(
                    "Shot Parent Object:",
                    shot.ShotParentGameObject,
                    typeof(GameObject),
                    allowSceneObjects: true);

                shot.StartTrigger = EditorGUILayout.IntSlider(
                    "Trigger " + Utility.Alphabet[shotIndex] + ":",
                    shot.StartTrigger,
                    leftValue: 0,
                    rightValue: 100);

                UpdatePreviousEndTriggerToStartTrigger(shotIndex);

                shot.EndTrigger = EditorGUILayout.IntSlider(
                    "Trigger " + Utility.Alphabet[shotIndex + 1] + ":",
                    shot.EndTrigger,
                    leftValue: 0,
                    rightValue: 100);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                shot.ShowPropsListControl = EditorGUILayout.Foldout(shot.ShowPropsListControl, "Number of Props:");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                int currentNumberOfProps = EditorGUILayout.IntField("\t", shot.PropsGameObjectList.Count);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    // TODO: This logic is used twice, simplify by moving to method
                    int countDifference = Mathf.Abs(currentNumberOfProps - shot.PropsGameObjectList.Count);
                    if (currentNumberOfProps > shot.PropsGameObjectList.Count)
                    {
                        for (int i = 0; i < countDifference; i++)
                            shot.PropsGameObjectList.Add(null);
                    }
                    else if (currentNumberOfProps < shot.PropsGameObjectList.Count)
                    {
                        if (currentNumberOfProps <= 0)
                            shot.PropsGameObjectList = new List<GameObject>();
                        else
                            shot.PropsGameObjectList.RemoveRange((shot.PropsGameObjectList.Count - 1) - countDifference, countDifference);
                    }
                }

                if (shot.ShowPropsListControl)
                {
                    for (int i = 0; i < shot.PropsGameObjectList.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(i + ": ");
                        shot.PropsGameObjectList[i] = (GameObject)EditorGUILayout.ObjectField(
                            shot.PropsGameObjectList[i],
                            typeof(GameObject),
                            allowSceneObjects: true);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            UpdateNextStartTriggerToEndTrigger(shotIndex);
        }
    }

    private void UpdatePreviousEndTriggerToStartTrigger(int shotIndex)
    {
        if (shotIndex > 0) // Excluding First shot
            shotManager.ShotsList[shotIndex - 1].EndTrigger = shotManager.ShotsList[shotIndex].StartTrigger;
    }

    private void UpdateNextStartTriggerToEndTrigger(int shotIndex)
    {
        ShotData shot = shotManager.ShotsList[shotIndex];
        if (shot.EndTrigger < shot.StartTrigger)
            shot.EndTrigger = shot.StartTrigger;

        if (shotIndex + 1 < shotManager.ShotsList.Count) // Excluding Last shot
            shotManager.ShotsList[shotIndex + 1].StartTrigger = shot.EndTrigger;
    }
}
