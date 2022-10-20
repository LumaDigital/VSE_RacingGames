public class Greyhounds : RacingGame
{
    private const int MaximumRacerCount = 12;

    public enum RaceTracks
    {
        Summerset_Park,
        Crawford_Park_Day,
        Crawford_Park_Night
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
                if (value == 7)
                    value = 8;
                else if (value == 9 || value == 10 || value == 11)
                    value = 12;
            }
            else // Decrement
            {
                if (value == 7)
                    value = 6;
                else if (value == 9 || value == 10 || value == 11)
                    value = 8;
            }
            numberOfRacers = VSEUtility.HandleMinMaxValues(value, MaximumRacerCount, MinimumRacerCount);
        }
    }
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
            "375", "520", "720"
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
            RaceTracks.Summerset_Park.ToString(),
            RaceTracks.Crawford_Park_Day.ToString(),
            RaceTracks.Crawford_Park_Night.ToString()
    };
}
