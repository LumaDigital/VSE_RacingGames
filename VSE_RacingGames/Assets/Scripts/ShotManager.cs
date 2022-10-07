using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour
{
    public List<ShotManagerData> ShotsList = new List<ShotManagerData>();

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
public class ShotManagerData
{
    public bool ShowDataControls;

    public int StartTrigger;
    public int EndTrigger;

    public GameObject ShotParent;
}
