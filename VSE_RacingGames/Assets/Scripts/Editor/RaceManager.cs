using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RaceManager : EditorWindow
{
    private readonly Type[] raceTypeOptions = {
        typeof(Thoroughbreds),
        typeof(Greyhounds),
        typeof(Harness),
        typeof(Cycling),
        typeof(SpeedSkating),
        typeof(IndyRacing),
        typeof(MotorRacing),
        typeof(SteepleChase)
    };
    private string[] raceTypeOptionsString;

    private bool showSceneDirectories;
    private int raceTypeIndex;
    private RacingGame currentRacingGameData;
    private Dictionary<string, string> raceTypeDirectories;

    [MenuItem("VSE/Race Manager")]
    public static void ShowWindow()
    {
        GetWindow<RaceManager>("VSE Race Manager");
    }

    private void OnEnable()
    {
        raceTypeOptionsString = raceTypeOptions.Select(element => element.Name).ToArray();
        RaceDataControls.AssignRacingGameData(ref currentRacingGameData, raceTypeOptions[raceTypeIndex]);
        raceTypeDirectories = VSEEditorUtility.RaceTypeDirectories;
    }

    private void OnGUI()
    {
        // Race Type Settings
        GUILayout.Space(VSEEditorUtility.MediumUISpacer);
        if (currentRacingGameData != null)
        {
            GUILayout.Label(" Handle and configure races", VSEEditorUtility.DescriptionLabelStyle);
            GUILayout.Space(VSEEditorUtility.MediumUISpacer);

            GUILayout.Label("Race Type:");
            EditorGUI.BeginChangeCheck();
            int previousRaceTypeIndex = raceTypeIndex;
            raceTypeIndex = EditorGUILayout.Popup(raceTypeIndex, raceTypeOptionsString);
            if (EditorGUI.EndChangeCheck())
            {
                if (RaceDataControls.HandleSceneReloadDialog())
                {
                    RaceDataControls.UpdateRacingGameType(ref currentRacingGameData, raceTypeOptions[raceTypeIndex]);
                    RaceDataControls.LoadTrackScene(ref currentRacingGameData, raceTypeOptions[raceTypeIndex]);
                }
                else
                    raceTypeIndex = previousRaceTypeIndex;
            }

            RaceDataControls.DrawRacingGameUI(ref currentRacingGameData, raceTypeOptions[raceTypeIndex]);
        }
        else
        {
            GUILayout.Label("ERROR:\nCurrent Race data is missing.\n" + "Generate/Find Race Data to continue.",
                VSEEditorUtility.ErrorLabelStyle);

            GUILayout.Space(VSEEditorUtility.SmallUISpacer);
            if (GUILayout.Button("Generate/Find Race Data"))
            {
                RaceDataControls.AssignRacingGameData(ref currentRacingGameData, raceTypeOptions[raceTypeIndex]);
                raceTypeIndex = raceTypeOptions.ToList().IndexOf(currentRacingGameData.GetType());
            }
        }

        // Scene Path Directory Setup
        GUILayout.Space(VSEEditorUtility.LargerUISpacer);
        showSceneDirectories = EditorGUILayout.Foldout(showSceneDirectories, "Scene Directories");

        for (int i = 0; i < raceTypeDirectories.Count; i++)
        {
            foreach (Type raceType in raceTypeOptions)
            {
                string currentPath = EditorPrefs.GetString(raceType.Name);
                if (raceType.Name == raceTypeDirectories.ElementAt(i).Key)
                {
                    raceTypeDirectories[raceType.Name] = currentPath;
                }
            }
        }

        if (showSceneDirectories)
        {
            for (int i = 0; i < raceTypeDirectories.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                raceTypeDirectories[raceTypeDirectories.ElementAt(i).Key] =
                    EditorGUILayout.TextField(raceTypeDirectories.ElementAt(i).Key + " Directory: ",
                        raceTypeDirectories.ElementAt(i).Value);

                if (GUILayout.Button("Set"))
                {
                    raceTypeDirectories[raceTypeDirectories.ElementAt(i).Key] =
                        EditorUtility.OpenFolderPanel(raceTypeDirectories.ElementAt(i).Key + " Path", 
                            "",
                            raceTypeDirectories.ElementAt(i).Value);
                }
                EditorGUILayout.EndHorizontal();
            }

            for (int i = 0; i < raceTypeDirectories.Count; i++)
            {
                if (raceTypeDirectories.ElementAt(i).Value != string.Empty)
                {
                    foreach (Type raceType in raceTypeOptions)
                    {
                        if (raceTypeDirectories.ElementAt(i).Value.Contains(raceType.Name))
                        {
                            EditorPrefs.SetString(raceType.Name, raceTypeDirectories.ElementAt(i).Value);
                        }
                    }
                }
            }
        }
    }
}