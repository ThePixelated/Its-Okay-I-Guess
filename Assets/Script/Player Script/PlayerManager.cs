using System;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public event Action<string> onInteractKey_E;
    public event Action<string> onTriggerEnter_non;
    public event Action onTriggerExit_non;
    public event Action onInteractKey_Dialogs;

    public void InteractKey_E(string objectName)
    {
        if (onInteractKey_E != null)
        {
            onInteractKey_E(objectName);
        }
    }

    public void TriggerEnter_non(string objectName)
    {
        if (onTriggerEnter_non != null)
        {
            onTriggerEnter_non(objectName);
        }
    }

    public void TriggerExit_non()
    {
        if (onTriggerExit_non != null)
        {
            onTriggerExit_non();
        }
    }

    public void InteractKey_Dialogs()
    {
        if (onInteractKey_Dialogs != null)
        {
            onInteractKey_Dialogs();
        }
    }
}
