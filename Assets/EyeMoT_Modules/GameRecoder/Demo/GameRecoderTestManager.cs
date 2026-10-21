using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EyeMoT;

public class GameRecoderTestManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer = 0f;
    private bool isStart = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {    
            StartRecording();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StopRecording();
        }

        if(isStart)
        {
            timer += Time.deltaTime;
            timerText.text = $"{timer:F2} sec";
        }
    }

    public void StartRecording()
    {
        GameRecoder.Instance.RecordStart();
        timer = 0f;
        isStart = true;
    }

    public void StopRecording()    {
        GameRecoder.Instance.RecordEnd();
        isStart = false;
    }
}
