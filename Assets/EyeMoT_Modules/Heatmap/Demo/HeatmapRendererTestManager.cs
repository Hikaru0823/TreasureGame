using System.Collections;
using System.Collections.Generic;
using EyeMoT;
using EyeMoT.Heatmap;
using UnityEngine;

public class HeatmapRendererTestManager : MonoBehaviour
{
    void Start()
    {
        HeatmapRenderer.Instance.VisibleHeatmap(true);
    }

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
