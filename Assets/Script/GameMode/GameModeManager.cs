using UnityEngine;

public class GameModeManager : MonoBehaviour
{

    [SerializeField] private GameObject m_PlayerObj;
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;

    private bool _keyPressedFlag = true;
    private bool _isEnableMove = false;
    private float _currentTime = 0f;

    public GameModeBase ExplorationMode = new ExplorationMode();
    public GameModeBase CardMode = new CardMode();
    public GameModeBase CurrentMode;

    private void Awake()
    {
        playerDirection = m_playerData.PlayerDirection;
        playerSpeed = m_playerData.PlayerSpeed;
    }

    private void Start()
    {
        CurrentMode = ExplorationMode;
        CurrentMode.Enter(this);
    }

    private void Update()
    {
        CurrentMode.Update(this);
    }

    public void HandleCharDirection()
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

    public void HandlePlayerMovement()
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
            Vector3 movement = new Vector3(m_PlayerObj.transform.position.x, m_PlayerObj.transform.position.y, m_PlayerObj.transform.position.z);
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

            m_PlayerObj.transform.position = movement;
            //Debug.Log(gameObject.transform.position);
        }
    }

    public void Switch(GameModeBase newMode)
    {
        CurrentMode.Exit(this);
        CurrentMode = newMode;
        CurrentMode.Enter(this);
    }
}
