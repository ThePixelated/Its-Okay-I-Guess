using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Transform[] anchorArea = { };
    [SerializeField] private string tagObj;

    private Direction _currentDirection;

    private void Start()
    {
        _currentDirection = m_playerData.PlayerDirection;
        SetTransform(0);
    }

    private void Update()
    {
        UpdateAnchor();
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == tagObj)
    //    {
    //    {
    //        //PlayerManager.Instance.TriggerEnter_non(collision.name);
    //        Debug.Log(gameObject.name + " - Enter... Mendeteksi " + collision.name);
    //        PlayerManager.Instance.InteractKey_E(collision.name);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == tagObj)
        {
            PlayerManager.Instance.TriggerEnter_non(collision.name);
            Debug.Log(gameObject.name + " - Enter... Mendeteksi " + collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == tagObj)
        {
            PlayerManager.Instance.TriggerExit_non();
            Debug.Log(gameObject.name + " - Exit... dari " + collision.name);
        }
    }

    private void SetTransform(int index)
    {
        gameObject.transform.localPosition = anchorArea[index].transform.localPosition;
        gameObject.transform.localRotation = anchorArea[index].transform.localRotation;
        gameObject.transform.localScale = anchorArea[index].transform.localScale;
    }

    private void UpdateAnchor()
    {
        int index = 0;
        switch (m_playerData.PlayerDirection)
        {
            case Direction.Forward:
                index = 0;
                break;
            case Direction.Left:
                index = 1;
                break;
            case Direction.Backward:
                index = 2;
                break;
            case Direction.Right:
                index = 3;
                break;
        }

        SetTransform(index);
    }
}
