using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Splines;

public abstract class RacingGame : MonoBehaviour
{
    public const string ScenarioDescription = "Scenarios refer to different starting sections for the race," +
        " such as starting the race on a straight or on a corner.\n\nScenarios vary per race type, distance" +
        " and location.";

    // Max and min number of racers vary per race type and modifier.
    // Some number of racers are excluded and these exlusions also vary.
    public abstract int NumberOfRacers { get; set; } // VSE TODO: Still need to spawn racers based on this
    public abstract int MinimumRacerCount { get; }
    public abstract string[] RaceDistanceOptions { get; }
    public abstract string[] RaceTrackOptions { get; }

    public int ScenarioIndex
    {
        get
        {
            return scenarioIndex;
        }
        set
        {
            scenarioIndex = value < 0 ? 0 : value;
        }
    }
    [SerializeField]
    private int scenarioIndex;

    public int RaceDistanceIndex
    {
        get
        {
            int index = RaceDistanceOptions.ToList().IndexOf(RaceDistance.ToString());
            return index < 0 ? 0 : index;
        }
    }

    public int RaceTrackIndex
    {
        get
        {
            int index = RaceTrackOptions.ToList().IndexOf(RaceTrackName);
            return index < 0 ? 0 : index;
        }
    }

    public SplineContainer CurrentRaceSplineContainer
    {
        get
        {
            foreach (SplineContainer splineContainer in FindObjectsOfType<SplineContainer>())
            {
                if (splineContainer.name.Contains(RaceDistance.ToString()))
                {
                    return splineContainer;
                }
            }

            VSEEditorUtility.LogVSEError("No Track Spline with " + RaceDistance + " in the name.\n" +
                "Ensure the selected distance has a corresponding Spline Container or change the distance.");

            return null;
        }
    }

    public int RaceDistance;
    public string RaceTrackName;
}

public abstract class ModifiableRacingGame : RacingGame
{
    public const string SilkDescription = "Silks refer to graphics representing the racers, and their UI icons." +
        "\nExample: Graphics for saddle cloths and jockey kits for thoroughbreds, or body paint for Motor racing.";

    public abstract Tuple<string, int>[] RaceModifierAndMaximumRacerCountArray { get; }

    public int RaceModifierIndex
    {
        get
        {
            int index = RaceModifierAndMaximumRacerCountArray.
                Select(element => element.Item1).ToList().IndexOf(RaceModifierName);
            return index < 0 ? 0 : index;
        }
    }

    public string RaceModifierName = Thoroughbreds.RaceModifiers.None.ToString();
}
