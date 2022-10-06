using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineTrigger : MonoBehaviour
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

    public GameObject objectToSpawn;
    public BoxTrigger[] boxTrigger;

    private void Update()
    {
        boxTrigger = GetComponentsInChildren<BoxTrigger>(true);
    }

    void SpawnOject()
    {
        Debug.Log("Method hit");
        if (objectToSpawn != null)
        {
            GameObject newInstance = Instantiate(objectToSpawn) as GameObject;
            newInstance.transform.SetParent(this.transform, false);
            newInstance.gameObject.SetActive(true);
        }
    }
}
