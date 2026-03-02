using UnityEngine;
using System.Runtime.InteropServices;
public class WebGLHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern bool IsMobileBrowser();
}

public class Platform
{
    public static bool IsMobileBrowser()
    {
#if UNITY_EDITOR
        return false; // value to return in Play Mode (in the editor)
#elif UNITY_WEBGL
    return WebGLHandler.IsMobileBrowser(); // value based on the current browser
#else
    return false; // value for builds other than WebGL
#endif
    }
}