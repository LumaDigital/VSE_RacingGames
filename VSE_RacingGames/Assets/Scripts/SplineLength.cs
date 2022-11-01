using System;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineLength : MonoBehaviour
{
    public SplineContainer SplineContainer
    {
        get
        {
            if (splineContainer == null)
                splineContainer = GetComponent<SplineContainer>();
            return splineContainer;
        }
    }
    private SplineContainer splineContainer;

    [ReadOnly]
    public float Length;

    void Update()
    {
        Length = (float)Math.Round(SplineContainer.CalculateLength(), 2);
    }
}