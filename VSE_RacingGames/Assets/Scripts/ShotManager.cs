using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour
{
    public List<ShotData> ShotsList = new List<ShotData>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class ShotData
{
    public bool ShowDataControls;
    public bool ShowPropsListControl;

    public int StartTrigger;
    public int EndTrigger;

    public GameObject ShotParentGameObject;

    public List<GameObject> PropsGameObjectList = new List<GameObject>();
}
