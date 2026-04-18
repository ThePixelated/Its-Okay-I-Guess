using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject cameraTopDown;
    [SerializeField] private GameObject cameraCard;

    public bool CameraCardIsLocked { get; set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        cameraTopDown.transform.position = new Vector3(player.position.x, player.position.y, cameraTopDown.transform.position.z);
    }

    public void ChangeActiveCamera(bool cameraTopDownState, bool cameraCardState)
    {
        cameraCard.SetActive(cameraCardState);
        // bisa dibuat trasisi ke kamera 3
        cameraTopDown.SetActive(cameraTopDownState);
    }
}
