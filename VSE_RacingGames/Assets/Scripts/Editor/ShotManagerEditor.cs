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
                    shotManager.ShotsList.Add(new ShotManagerData());
            }
            else if (currentNumberOfShots < shotManager.ShotsList.Count)
            {
                if (currentNumberOfShots <= 0)
                    shotManager.ShotsList = new List<ShotManagerData>();
                else
                    shotManager.ShotsList.RemoveRange((shotManager.ShotsList.Count - 1) - countDifference, countDifference);
            }
        }

        if (currentNumberOfShots > 0)
            DisplayShotData();
    }

    private void DisplayShotData()
    {
        foreach (ShotManagerData shot in shotManager.ShotsList)
        {
            int shotIndex = shotManager.ShotsList.IndexOf(shot);

            string shotName;
            if (shot.ShotParent != null)
                shotName = shot.ShotParent.name;
            else
                shotName = "Shot " + (shotIndex + 1);

            shot.ShowDataControls = EditorGUILayout.Foldout(shot.ShowDataControls, shotName);
            if (shot.ShowDataControls)
            {
                shot.StartTrigger = EditorGUILayout.IntSlider(
                    "Trigger " + Utility.Alphabet[shotIndex],
                    shot.StartTrigger,
                    leftValue: 0,
                    rightValue: 100);

                if (shotIndex > 0) // Excluding First shot
                    shotManager.ShotsList[shotIndex - 1].EndTrigger = shot.StartTrigger;

                shot.EndTrigger = EditorGUILayout.IntSlider(
                    "Trigger " + Utility.Alphabet[shotIndex + 1],
                    shot.EndTrigger,
                    leftValue: 0,
                    rightValue: 100);

                if (shot.EndTrigger < shot.StartTrigger)
                    shot.EndTrigger = shot.StartTrigger;

                if (shotIndex + 1 < shotManager.ShotsList.Count) // Excluding Last shot
                    shotManager.ShotsList[shotIndex + 1].StartTrigger = shot.EndTrigger;
            }
        }
    }
}
