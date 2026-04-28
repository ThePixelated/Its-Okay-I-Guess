using System;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    [SerializeField] private GameObject m_PlayerObj;
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private int playerSpeed;
    [SerializeField] private GameMode currentGameModeIndicator = GameMode.ExplorationMode;
    [SerializeField] private GameMode previousGameModeIndicator = GameMode.ExplorationMode;
    
    [SerializeField] private bool isObjectInteractable = false;
    private bool _isPaused = false;
    private bool _keyPressedFlag = true; // flag untuk movement
    private bool _isEnableMove = false; // flag untuk movement (efek rotate in-position)
    private float _currentTime = 0f;
    private string _interactableID;

    public string InteratableID { get { return _interactableID; } set {  _interactableID = value; }  }

    public GameModeBase ExplorationMode = new ExplorationMode();
    public GameModeBase DialogueMode = new DialogueMode();
    public GameModeBase TabletMode = new TabletMode();
    public GameModeBase CardMode = new CardMode();
    public GameModeBase CurrentMode;
    public GameModeBase PreviousMode;

    private void Awake()
    {
        Instance = this;

        playerDirection = m_playerData.PlayerDirection;
        playerSpeed = m_playerData.PlayerSpeed;
    }

    private void Start()
    {
        CurrentMode = ExplorationMode;
        PreviousMode = CurrentMode;
        CurrentMode.Enter(this);

        PlayerManager.Instance.onTriggerEnter_non += SetFlagTrue;
        PlayerManager.Instance.onTriggerExit_non += SetFlagFalse;
    }

    private void Update()
    {
        CurrentMode.Update(this);

        HandleSwitchMode();
        GMMPauseKeypadInputHandle();
    }

    public void GMMPauseKeypadInputHandle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P))
            HandleSwitchPauseMode();
    }

    public void GMMPauseClickHandle()
    {
        HandleSwitchPauseMode();
    }

    private void HandleSwitchPauseMode()
    {
        if (!_isPaused)
        {
            _isPaused = true;
            Switch(TabletMode);
        }
        else
        {
            _isPaused = false;
            Switch(PreviousMode);
        }
    }

    // masukin ke player controller, ambil reference scriptnya ke sini
    public void HandleCharDirection()
    {
        if (Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.UpArrow)))
            playerDirection = Direction.Forward;
        else if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow)))
            playerDirection = Direction.Left;
        else if (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow)))
            playerDirection = Direction.Backward;
        else if (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow)))
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
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // Forward
            {
                movement = new Vector3(movement.x, movement.y + playerSpeed * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // Left
            {
                movement = new Vector3(movement.x - playerSpeed * Time.deltaTime, movement.y, 0f);
            }

            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) // Backward
            {
                movement = new Vector3(movement.x, movement.y - playerSpeed * Time.deltaTime, 0f);
            }

            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) // Right
            {
                movement = new Vector3(movement.x + playerSpeed * Time.deltaTime, movement.y, 0f);
            }
            //Debug.Log(movement);

            m_PlayerObj.transform.position = movement;
            //Debug.Log(gameObject.transform.position);
        }
    }

    public void HandleObjectInteractable()
    {
        if (isObjectInteractable && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Key E Pressed...");

            PlayerManager.Instance.InteractKey_E(_interactableID);

            Switch(DialogueMode);
        }
    }

    //public void TempHandleQuest()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha0))
    //    {

    //    }
    //}

    public void HandleSwitchMode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraManager.Instance.CameraCardIsLocked = false;
            Switch(ExplorationMode);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Switch(CardMode);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Switch(DialogueMode);
        }
    }

    public void Switch(GameModeBase newMode)
    {
        PreviousMode = CurrentMode;
        CurrentMode.Exit(this);
        CurrentMode = newMode;
        CurrentMode.Enter(this);
    }

    public event Action<GameModeManager> onDialogueStop;
    public void DialogueStopped()
    {
        if (onDialogueStop != null)
        {
            onDialogueStop(this);
        }
    }

    public void HandleCameraChange(bool cameraTopDownState, bool cameraCardState)
    {
        CameraManager.Instance.ChangeActiveCamera(cameraTopDownState, cameraCardState);
    }

    public void SetFlagTrue(string objectName)
    {
        isObjectInteractable = true;
        _interactableID = objectName;
    }
    public void SetFlagFalse() => isObjectInteractable = false;

    public void SetCurrGameModeIndicator(GameMode modeIndicator) => currentGameModeIndicator = modeIndicator;

    public void SetPrevGameModeIndicator(GameMode modeIndicator) => previousGameModeIndicator = modeIndicator;
}


public enum GameMode
{
    ExplorationMode,
    DialogueMode,
    TabletMode,
    CardMode
}