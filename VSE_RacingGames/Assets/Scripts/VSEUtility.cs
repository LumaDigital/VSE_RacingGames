public class VSEUtility
{
    public static int HandleMinMaxValues(int currentValue, int maximumValue, int minimumValue = 0)
    {
        if (currentValue < minimumValue)
            currentValue = minimumValue;
        else if (currentValue > maximumValue)
            currentValue = maximumValue;

        return currentValue;
    }
}
