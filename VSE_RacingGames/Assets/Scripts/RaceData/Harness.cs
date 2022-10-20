public class Harness : RacingGame
{
    private const int MaximumRacerCount = 12;

    public enum RaceTracks
    {
        Westlake_Night,
        Westlake_Day
    }

    public override int NumberOfRacers
    {
        get
        {
            return numberOfRacers;
        }
        set
        {
            if (numberOfRacers < value) // Increment
            {
                if (value == 11)
                    value = 12;
            }
            else // Decrement
            {
                if (value == 11)
                    value = 10;
            }
            numberOfRacers = VSEUtility.HandleMinMaxValues(value, MaximumRacerCount, MinimumRacerCount);
        }
    }
    private int numberOfRacers;

    public override int MinimumRacerCount
    {
        get
        {
            return 10;
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
            "610", "700", "800"
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
            RaceTracks.Westlake_Night.ToString(),
            RaceTracks.Westlake_Day.ToString()
    };
}
