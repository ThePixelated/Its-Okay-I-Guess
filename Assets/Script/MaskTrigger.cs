using UnityEngine;

public class MaskTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering Mask side: " + gameObject.tag);
    }

    private void OnTriggerStay(Collider other)
    {
        GameEvents.instance.UIMaskTriggerStay(gameObject.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting Mask side: " + gameObject.tag);
        GameEvents.instance.UIMaskTriggerExit(gameObject.tag);
    }
}
