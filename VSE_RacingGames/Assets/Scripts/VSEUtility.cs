using UnityEngine;

public class VSEUtility
{
    public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static GUIStyle TriggerLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            style.normal.textColor = Color.white;

            return style;
        }
    }

    public static int HandleMinMaxValues(int currentValue, int maximumValue, int minimumValue = 0)
    {
        if (currentValue < minimumValue)
            currentValue = minimumValue;
        else if (currentValue > maximumValue)
            currentValue = maximumValue;

        return currentValue;
    }
}
