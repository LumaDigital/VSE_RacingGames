using UnityEngine;
using UnityEngine.Splines;

public class VSEUtility
{
    public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const float OnePercentFloat = 0.01f;

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

    public static void OrientTransformToSpline(Transform transform, SplineContainer splineContainer)
    {
        transform.position = splineContainer.EvaluatePosition(0);
        Vector3 lookDirection = (Vector3)splineContainer.EvaluatePosition(OnePercentFloat) - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
