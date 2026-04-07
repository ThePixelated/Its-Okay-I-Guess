using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLHandler : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern bool IsMobileBrowser();
    #endif
}

public class Platform
{
    public static bool IsMobileBrowser()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
                // Panggil fungsi JS cuma kalo di build WebGL asli
                return WebGLHandler.IsMobileBrowser(); 
        #else
                // Editor, Windows, Mac, dll balikannya false
                return false;
        #endif
    }
}