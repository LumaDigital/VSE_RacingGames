using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RaceDataControls
{
    private const string RaceDataGameObjectName = "Race Data";
    private const string ReloadSceneDialogTitle = "VSE: Load Race Scene";
    private const string ReloadSceneDialogMessage = "Changing Race Type/Track will require a scene reload.";

    public static void DrawRacingGameUI(ref RacingGame currentRacingGameData, Type raceType)
    {
        Undo.RecordObject(currentRacingGameData, nameof(currentRacingGameData.NumberOfRacers));
        currentRacingGameData.NumberOfRacers =
            EditorGUILayout.IntField("Number of Racers:", currentRacingGameData.NumberOfRacers);

        int raceTrackIndex = currentRacingGameData.RaceTrackIndex;
        EditorGUI.BeginChangeCheck();
        raceTrackIndex = EditorGUILayout.Popup("Race track:", raceTrackIndex, currentRacingGameData.RaceTrackOptions);
        if (EditorGUI.EndChangeCheck())
        {
            string extendedMessage = "\n\nSelected track: " + currentRacingGameData.RaceTrackOptions[raceTrackIndex];
            if (HandleSceneReloadDialog(extendedMessage))
            {
                currentRacingGameData.RaceTrackName = currentRacingGameData.RaceTrackOptions[raceTrackIndex];
                LoadTrackScene(ref currentRacingGameData, raceType);
            }
        }

        int raceDistanceIndex = currentRacingGameData.RaceDistanceIndex;
        EditorGUI.BeginChangeCheck();
        raceDistanceIndex = EditorGUILayout.Popup("Race distance:", raceDistanceIndex, currentRacingGameData.RaceDistanceOptions);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(currentRacingGameData, nameof(currentRacingGameData.RaceDistance));
            currentRacingGameData.RaceDistance = int.Parse(currentRacingGameData.RaceDistanceOptions[raceDistanceIndex]);
        }

        if (currentRacingGameData is ModifiableRacingGame modifiableRacingGame)
        {
            int raceModifierIndex = modifiableRacingGame.RaceModifierIndex;
            string[] raceModifierOptions =
                modifiableRacingGame.RaceModifierAndMaximumRacerCountArray.Select(element => element.Item1).ToArray();

            EditorGUI.BeginChangeCheck();
            raceModifierIndex = EditorGUILayout.Popup("Race modifier:", raceModifierIndex, raceModifierOptions);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentRacingGameData, nameof(modifiableRacingGame.RaceModifierName));
                modifiableRacingGame.RaceModifierName = raceModifierOptions[raceModifierIndex];
                if (modifiableRacingGame.RaceModifierName == Thoroughbreds.RaceModifiers.Lotto_Horses.ToString())
                {
                    modifiableRacingGame.RaceTrackName = currentRacingGameData.RaceTrackOptions[modifiableRacingGame.RaceTrackIndex];
                    if (EditorSceneManager.GetActiveScene().name != currentRacingGameData.RaceTrackName)
                    {
                        string extendedMessage = "\n\n" + modifiableRacingGame.RaceModifierName + " only has one track option: " +
                            modifiableRacingGame.RaceTrackName;
                        if (HandleSceneReloadDialog(extendedMessage))
                        {
                            LoadTrackScene(ref currentRacingGameData, raceType);

                            // The loaded scene doesn't have this modifier saved, so its reapplied
                            ((ModifiableRacingGame)currentRacingGameData).RaceModifierName = raceModifierOptions[raceModifierIndex];
                        }
                        else
                            Undo.PerformUndo();
                    }
                }
            }

            if (modifiableRacingGame.RaceModifierName == Thoroughbreds.RaceModifiers.Racing_Roulette.ToString())
            {
                Thoroughbreds thoroughbredsRacingGame = (Thoroughbreds)modifiableRacingGame;
                GUIContent silkGuiContent = new GUIContent("Silk set:", ModifiableRacingGame.SilkDescription);

                Undo.RecordObject(currentRacingGameData, nameof(thoroughbredsRacingGame.SilkSetIndex));
                thoroughbredsRacingGame.SilkSetIndex = EditorGUILayout.Popup(
                    silkGuiContent,
                    thoroughbredsRacingGame.SilkSetIndex,
                    thoroughbredsRacingGame.SilkSetOptions);
            }
        }

        GUIContent scenarioGuiContent =
            new GUIContent("Scenario index:", RacingGame.ScenarioDescription);

        Undo.RecordObject(currentRacingGameData, nameof(currentRacingGameData.ScenarioIndex));
        currentRacingGameData.ScenarioIndex =
            EditorGUILayout.IntField(scenarioGuiContent, currentRacingGameData.ScenarioIndex);
    }

    public static void AssignRacingGameData(ref RacingGame currentRacingGameData, Type raceType)
    {
        currentRacingGameData = GameObject.FindObjectOfType<RacingGame>();
        if (currentRacingGameData == null)
        {
            GameObject raceDataGameObject;
            if (GameObject.Find(RaceDataGameObjectName) is GameObject existingDataGameObject)
                raceDataGameObject = existingDataGameObject;
            else
                raceDataGameObject = new GameObject(RaceDataGameObjectName);

            Component racingGameComponent = raceDataGameObject.AddComponent(raceType);
            currentRacingGameData = racingGameComponent.GetComponent<RacingGame>();

            ApplyRacingGameDefaults(ref currentRacingGameData);
        }
    }

    public static void UpdateRacingGameType(ref RacingGame currentRacingGameData, Type raceType)
    {
        GameObject raceDataGameObject = currentRacingGameData.gameObject;
        GameObject.DestroyImmediate(currentRacingGameData);

        Component racingGameComponent = raceDataGameObject.AddComponent(raceType);
        currentRacingGameData = racingGameComponent.GetComponent<RacingGame>();

        ApplyRacingGameDefaults(ref currentRacingGameData);
    }

    public static bool HandleSceneReloadDialog(string extendedMessage = "")
    {
        return EditorUtility.DisplayDialog(
            ReloadSceneDialogTitle,
            ReloadSceneDialogMessage + extendedMessage + "\n\nDo you want to proceed?",
            ok: "Load Scene",
            cancel: "Cancel");
    }

    public static void LoadTrackScene(ref RacingGame currentRacingGameData, Type raceType)
    {
        string raceDirectory = string.Empty;
        foreach (KeyValuePair<string, string> raceTypeDirectory in VSEEditorUtility.RaceTypeDirectories)
        {
            if (raceTypeDirectory.Key.Equals(raceType.Name))
            {
                raceDirectory = raceTypeDirectory.Value;
            }
        }

        if (!Directory.Exists(raceDirectory))
        {
            LogMissingPathWarning(raceDirectory.Equals(string.Empty) ?
                "Directory not assigned" :
                raceDirectory);
            return;
        }

        string raceTrackPath = string.Empty;
        string[] raceSceneFiles = Directory.GetFiles(
            raceDirectory,
            "*" + VSEEditorUtility.UnitySceneExtension,
            SearchOption.AllDirectories);

        foreach (string raceSceneFile in raceSceneFiles)
        {
            if (raceSceneFile.Contains(currentRacingGameData.RaceTrackName))
            {
                raceTrackPath = raceSceneFile;
                break;
            }

            raceTrackPath = raceDirectory;
        }

        if (!File.Exists(raceTrackPath))
        {
            LogMissingPathWarning(raceDirectory.Equals(string.Empty) ?
                "Directory not assigned" :
                "No scene named '" + currentRacingGameData.RaceTrackName + "' in " + raceDirectory);
            return;
        }

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(raceTrackPath);
        AssignRacingGameData(ref currentRacingGameData, raceType);
    }

    private static void ApplyRacingGameDefaults(ref RacingGame currentRacingGameData)
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;
        if (!currentRacingGameData.RaceTrackOptions.Contains(sceneName))
            sceneName = currentRacingGameData.RaceTrackOptions[0];

        currentRacingGameData.RaceTrackName = sceneName;
        currentRacingGameData.RaceDistance = int.Parse(currentRacingGameData.RaceDistanceOptions[0]);
        currentRacingGameData.NumberOfRacers = currentRacingGameData.MinimumRacerCount;
        currentRacingGameData.ScenarioIndex = 0;

        if (currentRacingGameData is Thoroughbreds thoroughbredData)
        {
            thoroughbredData.RaceModifierName = thoroughbredData.RaceModifierAndMaximumRacerCountArray[0].Item1;
            thoroughbredData.SilkSetIndex = 0;
        }
    }

    private static void LogMissingPathWarning(string missingPath)
    {
        VSEEditorUtility.LogVSEWarning("'Missing path': " + missingPath + "\nDoes not exist, and requires implementation.");
    }
}

