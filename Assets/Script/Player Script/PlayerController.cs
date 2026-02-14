using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isObjectInteractable = false;

    private void Start()
    {
        PlayerManager.Instance.onTriggerEnter_non += SetFlagTrue;
        PlayerManager.Instance.onTriggerExit_non += SetFlagFalse;
    }

    void Update()
    {
        if (isObjectInteractable)
        {
            HandleObjectInteractable();
        }

        // target to dialog system, tapi bisa diakses kapan aca, bahaya ni
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("Dialog Interact Key Pressed...");
            PlayerManager.Instance.InteractKey_Dialogs();
        }
    }

    private void HandleObjectInteractable()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Key E Pressed...");
            isObjectInteractable = false;
            PlayerManager.Instance.InteractKey_E();
        }
    }

    public void SetFlagTrue() => isObjectInteractable = true;
    public void SetFlagFalse() => isObjectInteractable = false;
}
