using TMPro;
using UnityEngine;

public class DeviceDetection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deviceInfo;

    void Start()
    {
        if (deviceInfo != null)
        {
            if (Platform.IsMobileBrowser())
            {
                deviceInfo.text = "Device: Mobile detected - M";
            }
            else
            {
                deviceInfo.text = "Device: Desktop detected - D";
            }
        }
    }
}
