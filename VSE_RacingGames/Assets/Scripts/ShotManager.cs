using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

using CameraData;

[RequireComponent(typeof(SplineLength))]
public class ShotManager : MonoBehaviour
{
    public List<ShotData> ListOfShots = new List<ShotData>();
    public bool ToggleShotTriggerDisplay = true;
    public int NumberOfShots;

    private int triggerShotIndex;
    private bool hitLastTrigger;
    private float racerPosition;
    private bool requireComponent;

    public SplineLength SplineLength
    {
        get
        {
            if (splineLength == null)
                splineLength = GetComponent<SplineLength>();
            return splineLength;
        }
    }
    private SplineLength splineLength;

    public RacerDistance RacerDistance
    {
        get
        {
            if (racerDistance == null)
                racerDistance = GameObject.FindObjectOfType<RacerDistance>();
            return racerDistance;
        }
    }
    private RacerDistance racerDistance;

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

    public SplineAnimate SplineAnimate
    {
        get
        {
            if (splineAnimate == null)
                splineAnimate = GameObject.FindObjectOfType<SplineAnimate>();
            return splineAnimate;
        }
    }
    private SplineAnimate splineAnimate;

    private void Update()
    {
        if (RacerDistance != null && ListOfShots.Count > 0)
        {
            racerPosition = RacerDistance.splineLength * RacerDistance.splineNormalizedTime;
            if (racerPosition >= ListOfShots[triggerShotIndex].TriggerPosition && !hitLastTrigger)
            {
                ActivateShotCameraAndEntities(triggerShotIndex);

                if ((triggerShotIndex + 1) < ListOfShots.Count)
                    triggerShotIndex++;
                else
                    hitLastTrigger = true;
            }
        }
        else if (RacerDistance == null && requireComponent == false)
        {
            if ((EditorUtility.DisplayDialog("Racer Requires Missing Component!",
                "ShotManager requires the tracked racer to contain: " + typeof(RacerDistance),
                "Add Missing Component",
                "Cancel")))
            {
                SplineAnimate.gameObject.AddComponent(typeof(RacerDistance));
            }

            requireComponent = true;
        }
    }

    void ActivateShotCameraAndEntities(int triggerShotIndex)
    {
        if (triggerShotIndex > 0)
            DeactivatePreviousShotCameraAndEntities(triggerShotIndex);

        if (ListOfShots[triggerShotIndex].CameraComponent != null)
            ListOfShots[triggerShotIndex].CameraComponent.enabled = true;

        if (ListOfShots[triggerShotIndex].ListOfEntities.Count > 0)
        {
            foreach (GameObject entity in ListOfShots[triggerShotIndex].ListOfEntities)
            { 
                if (entity != null)
                    entity.SetActive(true);
                else
                    VSEEditorUtility.LogVSEWarning("Shot" + (triggerShotIndex + 1) + "'s Entity list contains an empty object!");
            }
        }
    }

    void DeactivatePreviousShotCameraAndEntities(int triggerShotIndex)
    {
        if (ListOfShots[triggerShotIndex - 1].CameraComponent != null)
            ListOfShots[triggerShotIndex - 1].CameraComponent.enabled = false;
        
        if (ListOfShots[triggerShotIndex - 1].ListOfEntities.Count > 0)
        {
            foreach (GameObject entity in ListOfShots[triggerShotIndex - 1].ListOfEntities)
            { 
                if (entity != null)
                    entity.SetActive(false);
                else
                    VSEEditorUtility.LogVSEWarning("Shot" + (triggerShotIndex) + "'s Entity list contains an empty object!");
            }
        }
    }
}

[Serializable]
public class ShotData
{
    public bool ToggleShotFoldout;
    public bool ToggleEntityFoldout = true;
    public float TriggerPosition;
    public float ShotTime;
    public GameObject TriggerObject;
    public RacingGameCamera CameraComponent;

    public List<GameObject> ListOfEntities = new List<GameObject>();
}