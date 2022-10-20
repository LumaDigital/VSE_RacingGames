using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class RaceManager : EditorWindow
{
    public Type[] RaceTypeOptions = new Type[]
    {
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

    private int raceTypeIndex;
    private RacingGame currentRacingGameData;

    [MenuItem("VSE/Race Manager")]
    public static void ShowWindow()
    {
        GetWindow<RaceManager>("VSE Race Manager");
    }

    private void OnEnable()
    {
        raceTypeOptionsString = RaceTypeOptions.Select(element => element.Name).ToArray();
        RaceDataControls.AssignRacingGameData(ref currentRacingGameData, RaceTypeOptions[raceTypeIndex]);
    }

    private void OnGUI()
    {
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
                    RaceDataControls.UpdateRacingGameType(ref currentRacingGameData, RaceTypeOptions[raceTypeIndex]);
                    RaceDataControls.LoadTrackScene(ref currentRacingGameData, RaceTypeOptions[raceTypeIndex]);
                }
                else
                    raceTypeIndex = previousRaceTypeIndex;
            }

            RaceDataControls.DrawRacingGameUI(ref currentRacingGameData, RaceTypeOptions[raceTypeIndex]);
        }
        else
        {
            GUILayout.Label("ERROR:\nCurrent Race data is missing.\n" + "Generate/Find Race Data to continue.",
                VSEEditorUtility.ErrorLabelStyle);

            GUILayout.Space(VSEEditorUtility.SmallUISpacer);
            if (GUILayout.Button("Generate/Find Race Data"))
            {
                RaceDataControls.AssignRacingGameData(ref currentRacingGameData, RaceTypeOptions[raceTypeIndex]);
                raceTypeIndex = RaceTypeOptions.ToList().IndexOf(currentRacingGameData.GetType());
            }
        }
    }
}