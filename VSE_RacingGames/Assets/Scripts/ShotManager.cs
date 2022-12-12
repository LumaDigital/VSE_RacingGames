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
    public bool RunRaceShots;
    public int NumberOfShots;

    private int triggerShotIndex;
    private bool hitLastTrigger;
    private float horsePosition;
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

    public HorseDistance HorseDistance
    {
        get
        {
            if (horseDistance == null)
                horseDistance = GameObject.FindObjectOfType<HorseDistance>();
            return horseDistance;
        }
    }
    private HorseDistance horseDistance;

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
        if (RunRaceShots)
            HandleShots();
    }

    private void HandleShots()
    {
        if (HorseDistance != null && ListOfShots.Count > 0)
        {
            horsePosition = HorseDistance.splineLength * HorseDistance.splineNormalizedTime;
            if (horsePosition >= ListOfShots[triggerShotIndex].TriggerPosition && !hitLastTrigger)
            {
                ActivateShotCameraAndEntities(triggerShotIndex);

                if ((triggerShotIndex + 1) < ListOfShots.Count)
                    triggerShotIndex++;
                else
                    hitLastTrigger = true;
            }
        }
        else if (HorseDistance == null && requireComponent == false)
        {
            if ((EditorUtility.DisplayDialog("Horse Requires Missing Component!",
                "ShotManager requires the tracked horse to contain: " + typeof(HorseDistance),
                "Add Missing Component",
                "Cancel")))
            {
                GameObject.Find("HorseAndJockey").AddComponent(typeof(HorseDistance));
            }

            requireComponent = true;
        }
    }

    private void ActivateShotCameraAndEntities(int triggerShotIndex)
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

    private void DeactivatePreviousShotCameraAndEntities(int triggerShotIndex)
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