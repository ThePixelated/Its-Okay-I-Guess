using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public event Action onInteractKey_E;
    public event Action onTriggerEnter_non;
    public event Action onTriggerExit_non;
    public event Action onInteractKey_Dialogs;

    public void InteractKey_E()
    {
        if (onInteractKey_E != null)
        {
            onInteractKey_E();
        }
    }

    public void TriggerEnter_non()
    {
        if (onTriggerEnter_non != null)
        {
            onTriggerEnter_non();
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
