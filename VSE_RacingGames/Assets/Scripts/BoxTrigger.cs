using System;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BoxTrigger : MonoBehaviour
{
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

    float min = 0f, max;
    float splineNormalizedTime;
    public float positionOnSpline;
    public List<GameObject> objectsToDeactivate;
    public List<GameObject> objectsToActivate;

    void Update()
    {
        max = SplineContainer.CalculateLength();
        float position = Mathf.Clamp(positionOnSpline, min, max);
        positionOnSpline = position;
        splineNormalizedTime = position / max;

        this.transform.position = SplineContainer.EvaluatePosition(splineNormalizedTime);
        this.transform.LookAt(SplineContainer.EvaluatePosition(splineNormalizedTime + (float)0.001));
    }

    void DeactivateObjects()
    {
        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }
    }

    void ActivateObjects()
    {
        foreach (var item in objectsToActivate)
        {
            item.SetActive(true);
        }
    }
}
