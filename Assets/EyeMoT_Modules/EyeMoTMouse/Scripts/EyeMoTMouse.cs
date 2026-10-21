using UnityEngine;
using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace EyeMoT
{
    public class EyeMoTMouse : SceneSingleton<EyeMoTMouse>
    {
        [Header("Resources")]
        [SerializeField] private Sprite[] icons = default; // 0: off, 1: on
        [SerializeField] private Button gazeButton = default;
        [SerializeField] private Image gazeImage = default;

        private Image gazeButtonImage = default;
        private Keyboard keyboard = default;
        private Process cmdProcess = default;

        private bool isOn = false;

        void Start()
        {
            #if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            this.gazeButton.interactable = false;
            return;
            #endif
            Process[] eyeMoTProceses = Process.GetProcessesByName("EyeMoTMouse");

            if (eyeMoTProceses.Length > 0)
            {
                foreach (Process eyeMoTProcess in eyeMoTProceses)
                    eyeMoTProcess.Kill();
            }

            if (this.cmdProcess == null)
            {
                this.cmdProcess = new Process();

                this.cmdProcess.StartInfo.FileName = Application.dataPath + "/../EyeMoTMouse/EyeMoTMouse.exe";

                this.cmdProcess.StartInfo.Arguments = "30";

                this.cmdProcess.EnableRaisingEvents = true;

                this.cmdProcess.Exited += CmdProcessExited;

                this.cmdProcess.StartInfo.UseShellExecute = false;

                this.cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                this.cmdProcess.StartInfo.RedirectStandardOutput = true;

                this.cmdProcess.StartInfo.RedirectStandardInput = true;

                this.cmdProcess.OutputDataReceived += OutputHandler;

                try
                {
                    this.cmdProcess.Start();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError("<color=orange>[EyeMoTMouse]</color> EyeMoTMouse.exe is not found. Please check the path. " + e.Message);
                    this.cmdProcess.Dispose();
                    this.cmdProcess = null;
                    this.gazeButton.interactable = false;
                    return;
                }

                this.cmdProcess.BeginOutputReadLine();

                this.gazeButtonImage = this.gazeImage;
                this.keyboard = Keyboard.current;

                this.OnStatusChanged(!this.isOn);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.keyboard != null)
            {
                if (this.keyboard.xKey.wasReleasedThisFrame)
                    this.OnStatusChanged(!this.isOn);
            }
        }

        // EyeMoTMouse
        private void OutputHandler(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                string trimedArgs = args.Data.Trim();

                switch (trimedArgs)
                {
                    case "0":

                        return;

                    case "1":

                        return;

                    case "StartUp":

                        return;
                }
            }
        }

        void CmdProcessExited(object sender, System.EventArgs e)
        {
            this.cmdProcess.Dispose();
            this.cmdProcess = null;
        }

        private void OnApplicationQuit()
        {
            #if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return;
            #endif
            if (this.cmdProcess != null)
            {
                this.cmdProcess.StandardInput.WriteLine("exit");

                this.cmdProcess.WaitForExit(1500);
                this.cmdProcess.Kill();
                this.cmdProcess.Dispose();
                this.cmdProcess = null;
            }
        }

        public void OnButtonClicked()
        {
            #if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return;
            #endif
            this.OnStatusChanged(!this.isOn);
        }

        private void OnStatusChanged(bool isOn)
        {
            #if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return;
            #endif
            UnityEngine.Debug.Log("<color=orange>[EyeMoTMouse]</color> EyeMoTMouse is turned " + (isOn ? "on." : "off."));
            this.cmdProcess.StandardInput.WriteLine(isOn ? "mouse_off" : "mouse_on");
            this.gazeButtonImage.sprite = this.icons[isOn ? 1 : 0];
            this.isOn = isOn;
        }
    }
}
