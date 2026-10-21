using System.Collections;
using System.Collections.Generic;
using KanKikuchi.AudioManager;
using UnityEngine;

public class AudioTestManager : MonoBehaviour
{
    public void BGMStart()
    {
        BGMManager.Instance.Play(BGMPath.FANTASY14);
    }

    public void SEStart()
    {
        SEManager.Instance.Play(SEPath.SYSTEM23);
    }
}
