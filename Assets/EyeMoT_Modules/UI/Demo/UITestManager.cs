using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyeMoT;
public class UITestManager : MonoBehaviour
{
    public void VisiblePopUp()
    {
        PopupUI.OnVisible("Test Title", "Test Description", PopupUI.Type.Alert);
    }
}
