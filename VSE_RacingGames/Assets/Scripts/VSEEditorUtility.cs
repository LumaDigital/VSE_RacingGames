using UnityEngine;

public static class VSEEditorUtility
{
    public const string UnitySceneExtension = ".unity";

    private const string WarningPrefix = "|| VSE WARNING || ";

    public const float LargeUISpacer = 20f;
    public const float MediumUISpacer = LargeUISpacer / 2;
    public const float SmallUISpacer = (LargeUISpacer / 4) + 1;

    public static Color32 DescriptionLabelColour = new Color32(96, 219, 127, 255);

    public static GUIStyle DescriptionLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 25;
            style.normal.textColor = DescriptionLabelColour;

            return style;
        }
    }

    public static GUIStyle ErrorLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 15;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;

            return style;
        }
    }

    public static GUILayoutOption ThreeDigitWidthLayoutOption
    {
        get
        {
            return GUILayout.MaxWidth(37);
        }
    }

    public static void LogVSEWarning(string errorMessage)
    {
        Debug.LogWarning(WarningPrefix + errorMessage);
    }

    public static void RoundOffToOneDecimals(ref float value)
    {
        value = (Mathf.Round(value * 10)) / 10;
    }
}
