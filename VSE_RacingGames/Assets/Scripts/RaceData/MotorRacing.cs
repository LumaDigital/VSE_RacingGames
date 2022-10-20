using System;
using System.Linq;

public class MotorRacing : ModifiableRacingGame
{
    public enum RaceModifiers
    {
        None,
        Lotto_Cars
    }

    public enum RaceTracks
    {
        Atlanta_Ring
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
            new Tuple<string, int>(RaceModifiers.Lotto_Cars.ToString(), 14)
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
                    // Races after the first index ie "Modfied Races" always have the same number of racers
                    if (RaceModifierAndMaximumRacerCountArray.First() == modifierAndMaximumRacerCount)
                    {
                        if (numberOfRacers < value) // Increment
                        {
                            if (value == 9)
                                value = 10;
                            else if (value == 11)
                                value = 12;
                            else if (value == 13)
                                value = 14;
                        }
                        else // Decrement
                        {
                            if (value == 9)
                                value = 8;
                            else if (value == 11)
                                value = 10;
                            else if (value == 13)
                                value = 12;
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
    private int numberOfRacers;

    public override int MinimumRacerCount
    {
        get
        {
            return 8;
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
            "5000"
    };

    public override string[] RaceTrackOptions
    {
        get
        {
            return raceTrackOptions;
        }
    }
    private string[] raceTrackOptions = new string[]
    {
            RaceTracks.Atlanta_Ring.ToString()
    };
}
