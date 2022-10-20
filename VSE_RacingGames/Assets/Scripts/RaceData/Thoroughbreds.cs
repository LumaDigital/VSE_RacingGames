using System;
using System.Linq;

using UnityEngine;

public class Thoroughbreds : ModifiableRacingGame
{
    public enum RaceModifiers
    {
        None,
        Racing_Roulette,
        Lotto_Horses
    }

    private enum RaceTracks
    {
        Bakersfield,
        Milnerton,
        PalmRidge
    }

    private enum SilkSets
    {
        Generic,
        Dominican_Republic
    }

    public override Tuple<string, int>[] RaceModifierAndMaximumRacerCountArray
    {
        get
        {
            return raceModifierAndMaximumRacerCount;
        }
    }
    private Tuple<string, int>[] raceModifierAndMaximumRacerCount = new Tuple<string, int>[]
    {
            new Tuple<string, int>(RaceModifiers.None.ToString(), 14),
            new Tuple<string, int>(RaceModifiers.Racing_Roulette.ToString(), 13),
            new Tuple<string, int>(RaceModifiers.Lotto_Horses.ToString(), 16)
    };

    public override int NumberOfRacers
    {
        get
        {
            return numberOfRacers;
        }
        set
        {
            foreach (Tuple<string, int> modifierAndMaximumRacerCount in RaceModifierAndMaximumRacerCountArray)
            {
                if (modifierAndMaximumRacerCount.Item1 == RaceModifierName)
                {
                    // Races after the first index ie "Modified Races" always have the same number of racers
                    if (RaceModifierAndMaximumRacerCountArray.First() == modifierAndMaximumRacerCount)
                    {
                        if (numberOfRacers < value) // Increment
                        {
                            if (value == 7)
                                value = 8;
                        }
                        else // Decrement
                        {
                            if (value == 7)
                                value = 6;
                        }

                        numberOfRacers = VSEUtility.HandleMinMaxValues(
                            value,
                            modifierAndMaximumRacerCount.Item2,
                            MinimumRacerCount);
                    }
                    else
                    {
                        numberOfRacers = VSEUtility.HandleMinMaxValues(
                            value, modifierAndMaximumRacerCount.Item2, modifierAndMaximumRacerCount.Item2);
                    }

                    break;
                }
            }
        }
    }
    [SerializeField]
    private int numberOfRacers;

    public override int MinimumRacerCount
    {
        get
        {
            return 6;
        }
    }

    public override string[] RaceDistanceOptions
    {
        get
        {
            return distanceOptions;
        }
    }
    private string[] distanceOptions = new string[]
    {
            "700", "800", "1000", "1600", "2000"
    };

    public override string[] RaceTrackOptions
    {
        get
        {
            if (RaceModifierName == RaceModifiers.Lotto_Horses.ToString())
                return new string[] { RaceTracks.Milnerton.ToString() };
            else
                return raceTrackOptions;
        }
    }
    private string[] raceTrackOptions = new string[]
    {
            RaceTracks.Bakersfield.ToString(),
            RaceTracks.Milnerton.ToString(),
            RaceTracks.PalmRidge.ToString()
    };

    public string[] SilkSetOptions = new string[]
    {
            SilkSets.Generic.ToString(),
            SilkSets.Dominican_Republic.ToString()
    };

    public int SilkSetIndex;
}
