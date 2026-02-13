using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;

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
        Vector2 movement = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        if (Input.GetKey(KeyCode.W)) // Forward
        {
            movement = new Vector2(movement.x, movement.y + playerSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A)) // Left
        {
            movement = new Vector2(movement.x - playerSpeed * Time.deltaTime, movement.y);
        }

        else if (Input.GetKey(KeyCode.S)) // Backward
        {
            movement = new Vector2(movement.x, movement.y - playerSpeed * Time.deltaTime);
        }

        else if (Input.GetKey(KeyCode.D)) // Right
        {
            movement = new Vector2(movement.x + playerSpeed * Time.deltaTime, movement.y);
        }

        gameObject.transform.position = movement;
    }
}


