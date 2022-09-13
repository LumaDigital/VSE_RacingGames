using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineDisplay : MonoBehaviour
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

    [SerializeField]
    private bool splineGizmoDisplay = true;
    [SerializeField]
    private float calculationIncrement = 0.01f;

    void OnDrawGizmosSelected()
    {
        if (calculationIncrement > 0.0f & splineGizmoDisplay)
        {
            for (float i = 0f; i < 1f; i += calculationIncrement)
                Gizmos.DrawLine(SplineContainer.EvaluatePosition(i), SplineContainer.EvaluatePosition(i + calculationIncrement));           
        }
        else if (calculationIncrement <= 0.0f & splineGizmoDisplay)
            Debug.LogError("Spline Gizmo draw failed! Calculation Increment must be between 0 and 1. Currently: " + calculationIncrement);        
    }
}