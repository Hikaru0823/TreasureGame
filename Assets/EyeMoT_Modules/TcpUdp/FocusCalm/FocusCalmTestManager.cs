using System.Collections;
using System.Collections.Generic;
using System;
using EyeMoT;
using UnityEngine;

public class FocusCalmTestManager : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            RecordManager.Instance.StartRecord(true, true);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            RecordManager.Instance.StopRecord();
        }
    }
}
