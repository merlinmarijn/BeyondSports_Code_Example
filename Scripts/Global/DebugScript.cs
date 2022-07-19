using UnityEngine;
using Photon.Pun;

namespace smashclone
{
    public class DebugScript : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;
    //    private float deltaTime;

    // void Update () {
    //     deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    //     float fps = 1.0f / deltaTime;
    //     Debug.Log("FPS: " + fps);
    // }

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                myLog = GUI.TextArea(new Rect(10, 10, Screen.width/2, Screen.height/2), myLog);
            }
        }
        //#endif
    }
}