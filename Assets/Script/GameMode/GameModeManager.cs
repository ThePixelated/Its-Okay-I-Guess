using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    [SerializeField] private GameObject m_PlayerObj;
    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private Direction playerDirection;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float footstepSpeed = .5f;
    [SerializeField] private GameMode currentGameModeIndicator = GameMode.ExplorationMode;
    [SerializeField] private GameMode previousGameModeIndicator = GameMode.ExplorationMode;

    [SerializeField] private bool isObjectInteractable = false;

    private bool _isPaused = false;
    private bool _keyPressedFlag = true; // flag untuk movement
    private bool _isEnableMove = false; // flag untuk movement (efek rotate in-position)
    private float _currentTime = 0f;
    private string _interactableID;

    // PLAYER CONFIG
    private Rigidbody2D _rb;
    private Vector2 _moveinput;
    private Animator _playerAnim;
    private bool _playingFootsteps = false;


    public string InteratableID { get { return _interactableID; } set { _interactableID = value; } }

    public GameModeBase ExplorationMode = new ExplorationMode();
    public GameModeBase TransitionMode = new TransitionMode();
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

        CurrentMode = TransitionMode;
        PreviousMode = CurrentMode;
    }

    private void Start()
    {
        CurrentMode.Enter(this);

        PlayerManager.Instance.onTriggerEnter_non += SetFlagTrue;
        PlayerManager.Instance.onTriggerExit_non += SetFlagFalse;

        _rb = m_PlayerObj.GetComponent<Rigidbody2D>();
        _playerAnim = m_PlayerObj.GetComponent<Animator>();
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
        {
            playerDirection = Direction.Forward;
            //m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow)))
        {
            playerDirection = Direction.Left;
            //m_PlayerObj.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow)))
        {
            playerDirection = Direction.Backward;
            //m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow)))
        {
            playerDirection = Direction.Right;
            //m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
        }

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
            ////Debug.LogWarning("MOVEMENT OBSERVE");
            //Vector3 movement = new Vector3(m_PlayerObj.transform.position.x, m_PlayerObj.transform.position.y, m_PlayerObj.transform.position.z);
            ////Debug.Log(movement);
            //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // Forward
            //{
            //    movement = new Vector3(movement.x, movement.y + playerSpeed * Time.deltaTime, 0f);
            //}
            //else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // Left
            //{
            //    movement = new Vector3(movement.x - playerSpeed * Time.deltaTime, movement.y, 0f);
            //}

            //else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) // Backward
            //{
            //    movement = new Vector3(movement.x, movement.y - playerSpeed * Time.deltaTime, 0f);
            //}

            //else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) // Right
            //{
            //    movement = new Vector3(movement.x + playerSpeed * Time.deltaTime, movement.y, 0f);
            //}
            ////Debug.Log(movement);

            
            //Debug.Log(gameObject.transform.position);
        }

        _rb.linearVelocity = _moveinput * playerSpeed;
        _playerAnim.SetBool("isWalking", _rb.linearVelocity.magnitude > 0);

        if (_rb.linearVelocity.magnitude > 0 && !_playingFootsteps)
        {
            Debug.LogWarning("Sounds PLAYED");
            StartFootstep();
        }
        else if (_rb.linearVelocity.magnitude == 0)
        {
            PlayerConfigExit();
        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (CurrentMode == ExplorationMode)
        {
            _playerAnim.SetBool("isWalking", true);

            if (context.canceled)
            {
                _playerAnim.SetBool("isWalking", false);
                _playerAnim.SetFloat("lastInputX", _moveinput.x);
                _playerAnim.SetFloat("lastInputY", _moveinput.y);

                if (_moveinput.x > 0)
                {
                    m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
                }
                else if (_moveinput.x < 0)
                {
                    m_PlayerObj.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (_moveinput.y < 0)
                {
                    m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
                }

                //_rb.linearVelocity = Vector2.zero;
                //StopFootstep();
            }

            _moveinput = context.ReadValue<Vector2>();
            _playerAnim.SetFloat("inputX", _moveinput.x);
            _playerAnim.SetFloat("inputY", _moveinput.y);

            if (_moveinput.x > 0)
            {
                m_PlayerObj.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (_moveinput.x < 0)
            {
                m_PlayerObj.GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (_moveinput.y < 0)
            {
                m_PlayerObj.GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        else
        {
            PlayerConfigExit();
            return;
        }
    }

    private void StartFootstep()
    {
        _playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    private void StopFootstep()
    {
        _playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    public void PlayerConfigExit()
    {
        _rb.linearVelocity = Vector2.zero;
        _playerAnim.SetBool("isWalking", false);
        _moveinput = Vector2.zero;
        StopFootstep();
    }

    private void PlayFootstep()
    {
        SoundEffectManager.Play("PlayerFootstep", true);
    }

    public void HandleObjectInteractable()
    {
        if (isObjectInteractable && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Key E Pressed...");

            PlayerManager.Instance.InteractKey_E(_interactableID);
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

    public event Action<GameModeManager> onTransitionStop;
    public void TransitionStop()
    {
        if (onTransitionStop != null)
        {
            onTransitionStop(this);
        }
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
    TransitionMode,
    DialogueMode,
    TabletMode,
    CardMode
}