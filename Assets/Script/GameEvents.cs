using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }


    public event Action<string> onUIMaskTriggerStay;
    public void UIMaskTriggerStay(string tag)
    {
        if (onUIMaskTriggerStay != null)
        {
            onUIMaskTriggerStay(tag);
        }
    }

    public event Action<string> onUIMaskTriggerExit;
    public void UIMaskTriggerExit(string tag)
    {
        if (onUIMaskTriggerExit != null)
        {
            onUIMaskTriggerExit(tag);
        }
    }

}
