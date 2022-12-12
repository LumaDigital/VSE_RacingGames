using UnityEditor;
using UnityEngine;

public static class VSEEditorUtility
{
    public const string UnitySceneExtension = ".unity";

    private const string NotificationPrefix = "|| VSE NOTIFICATION || ";
    private const string WarningPrefix = "|| VSE WARNING || ";
    private const string ErrorPrefix = "|| VSE ERROR || ";

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
            style.wordWrap = true;

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
            style.wordWrap = true;

            return style;
        }
    }

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

    public static GUILayoutOption ThreeDigitWidthLayoutOption
    {
        get
        {
            return GUILayout.MaxWidth(37);
        }
    }

    public static void LogVSENotification(string errorMessage)
    {
        Debug.Log(NotificationPrefix + errorMessage);
    }

    public static void LogVSEWarning(string errorMessage)
    {
        Debug.LogWarning(WarningPrefix + errorMessage);
    }

    public static void LogVSEError(string errorMessage)
    {
        Debug.LogError(ErrorPrefix + errorMessage);
        EditorApplication.isPlaying = false;
    }

    public static void RoundOffToOneDecimals(ref float value)
    {
        value = (Mathf.Round(value * 10)) / 10;
    }
}
