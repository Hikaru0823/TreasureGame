using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EyeMoT
{
    public class ExitManager : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private GameObject _exitButton;

        void Start()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            _exitButton.SetActive(false);
            #endif
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        public void QuitApplication()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBGL
            
            #else            
            Application.Quit();
            #endif
        }
    }
}