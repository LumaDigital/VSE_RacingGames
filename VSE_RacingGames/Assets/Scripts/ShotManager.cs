using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

public class ShotManager : MonoBehaviour
{
    public List<ShotData> ListOfShots = new List<ShotData>();
    public bool ToggleShotTriggerDisplay = true;
    public bool hitLastTrigger = false;
    public int TriggerShotIndex;
    private float lookAtValue = 0.01f;
    public float Length;
    public float HorsePosition;

    public int NumberOfShots;

    public HorseDistance HorseDistance
    {
        get
        {
            if (horseDistance == null)
                horseDistance = GameObject.Find("HorseAndJockey").GetComponent<HorseDistance>();
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

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.white;

        /*foreach (ShotData shot in ListOfShots)
        {
            if (ToggleShotTriggerDisplay)
            {
                int shotIndex = ListOfShots.IndexOf(shot);

                if (shot.TriggerObject == null)
                {
                    shot.TriggerObject = new GameObject("ShotTrigger " + (shotIndex + 1));
                    shot.TriggerObject.transform.parent = transform;
                    shot.TriggerObject.AddComponent(typeof(ShotTrigger));
                }

                // Move to method in ShotManagerEditor
                shot.TriggerObject.transform.position = SplineContainer.EvaluatePosition(shot.TriggerPosition / Length);
                shot.TriggerObject.transform.LookAt(SplineContainer.EvaluatePosition(shot.TriggerPosition / Length + lookAtValue));

                // Move to ShotTrigger OnDrawGizmos
                Handles.Label(shot.TriggerObject.transform.position, Utility.Alphabet[shotIndex].ToString(), style);
            }
        }*/
    }

    private void Update()
    {
        Debug.Log(HorseDistance);
        HorsePosition = (float)Math.Round(horseDistance.splineLength * horseDistance.splineNormalizedTime, 0);

        if (HorsePosition == ListOfShots[TriggerShotIndex].TriggerPosition && !hitLastTrigger)
            {
                Debug.Log(ListOfShots[TriggerShotIndex].TriggerPosition);

                ActivateShotCameraAndEntities(TriggerShotIndex);

                if ((TriggerShotIndex + 1) < ListOfShots.Count)
                    TriggerShotIndex++;
                else
                    hitLastTrigger = true;
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
                entity.SetActive(true);
        }
    }

    void DeactivatePreviousShotCameraAndEntities(int triggerShotIndex)
    {
        if (ListOfShots[triggerShotIndex - 1].CameraComponent != null)
            ListOfShots[triggerShotIndex - 1].CameraComponent.enabled = false;
        
        if (ListOfShots[triggerShotIndex - 1].ListOfEntities.Count > 0)
        {
            foreach (GameObject entity in ListOfShots[triggerShotIndex - 1].ListOfEntities)
                entity.SetActive(false);
        }
    }
}

// Don't need this to be serialized I reckon.
//[Serializable]
public class ShotData
{
    public bool ToggleShotFoldout;
    public bool ToggleEntityFoldout = true;
    public int TriggerPosition;
    public GameObject TriggerObject;
    public Camera CameraComponent;

    public List<GameObject> ListOfEntities = new List<GameObject>();
}