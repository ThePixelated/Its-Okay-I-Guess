using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;

    private bool _keyPressedFlag = true;
    private bool _isEnableMove = false;
    private float _currentTime = 0f;

    private void Awake()
    {
        playerDirection = m_playerData.PlayerDirection;
        playerSpeed = m_playerData.PlayerSpeed;
    }

    private void Update()
    {
        HandleCharDirection();
        HandlePlayerMovement();
    }

    private void HandleCharDirection()
    {
        if (Input.GetKey(KeyCode.W))
            playerDirection = Direction.Forward;
        else if (Input.GetKey(KeyCode.A))
            playerDirection = Direction.Left;
        else if (Input.GetKey(KeyCode.S))
            playerDirection = Direction.Backward;
        else if (Input.GetKey(KeyCode.D))
            playerDirection = Direction.Right;

        m_playerData.PlayerDirection = playerDirection;
    }

    private void HandlePlayerMovement()
    {
        if (Input.anyKey && _keyPressedFlag)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= 0.13f)
            {
                _isEnableMove = true;
                _keyPressedFlag = false;
            }
        }
        else if (!Input.anyKey && !_keyPressedFlag)
        {
            _keyPressedFlag = true;
            _isEnableMove = false;
            _currentTime = 0f;
        }

        if (_isEnableMove)
        {
            //Debug.LogWarning("MOVEMENT OBSERVE");
            Vector3 movement = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            //Debug.Log(movement);
            if (Input.GetKey(KeyCode.W)) // Forward
            {
                movement = new Vector3(movement.x, movement.y + playerSpeed * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.A)) // Left
            {
                movement = new Vector3(movement.x - playerSpeed * Time.deltaTime, movement.y, 0f);
            }

            else if (Input.GetKey(KeyCode.S)) // Backward
            {
                movement = new Vector3(movement.x, movement.y - playerSpeed * Time.deltaTime, 0f);
            }

            else if (Input.GetKey(KeyCode.D)) // Right
            {
                movement = new Vector3(movement.x + playerSpeed * Time.deltaTime, movement.y, 0f);
            }
            //Debug.Log(movement);

            gameObject.transform.position = movement;
            //Debug.Log(gameObject.transform.position);
        }
    }
}