public class SpeedSkating : RacingGame
{
    private const int MaximumRacerCount = 6;

    public enum RaceTracks
    {
        Beijing
    }

    public override int NumberOfRacers
    {
        get
        {
            return numberOfRacers;
        }
        set
        {
            numberOfRacers = VSEUtility.HandleMinMaxValues(value, MaximumRacerCount, MinimumRacerCount);
        }
    }
    private int numberOfRacers;

    public override int MinimumRacerCount
    {
        get
        {
            return MaximumRacerCount;
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
            "500"
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
            RaceTracks.Beijing.ToString()
    };
}
