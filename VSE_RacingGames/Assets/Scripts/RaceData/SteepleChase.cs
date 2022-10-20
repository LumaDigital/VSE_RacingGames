public class SteepleChase : RacingGame
{
    private const int MaximumRacerCount = 14;

    public enum RaceTracks
    {
        Milnerton
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
            numberOfRacers = VSEUtility.HandleMinMaxValues(value, MaximumRacerCount, MinimumRacerCount);
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
            "700", "800", "1000", "1600", "2000"
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
            RaceTracks.Milnerton.ToString()
    };
}
