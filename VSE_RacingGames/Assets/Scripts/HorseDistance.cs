using System;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class HorseDistance : MonoBehaviour
{
    public SplineAnimate SplineAnimate
    {
        get
        {
            if (splineAnimate == null)
                splineAnimate = GetComponent<SplineAnimate>();
            return splineAnimate;
        }
    }
    private SplineAnimate splineAnimate;

    public SplineContainer SplineContainer
    {
        get
        {
            if (splineContainer == null)
                splineContainer = FindObjectOfType<SplineContainer>();
            return splineContainer;
        }
    }
    private SplineContainer splineContainer;

    [SerializeField][ReadOnly]
    string percentCompleted;
    [SerializeField][ReadOnly]
    string distanceCompleted;
    float splineLength;
    float splineNormalizedTime;

    void Update()
    {
        splineLength = SplineContainer.CalculateLength();
        splineNormalizedTime = SplineAnimate.normalizedTime;

        distanceCompleted = Math.Round((splineLength * splineNormalizedTime), 2).ToString() + " / " + Math.Round(splineLength, 2);
        percentCompleted = Math.Round((splineNormalizedTime * 100), 2).ToString() + "%";
    }
}