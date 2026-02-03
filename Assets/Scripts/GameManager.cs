using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.Playing;

    public static event Action<GameState> OnStateChanged;
    public static event Action<int, Vector3> OnCheckpointReached;
    public static event Action<Vector3> OnFinalCheckpointReached;
    public static event Action<int> OnExitReached;

    [SerializeField] private int totalCheckpoints = 5;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Player player;
    
    [Header("Tilemap")]
    [SerializeField] private GameObject destroyableTilemap;
    [SerializeField] private GameObject destroyedObstacles;

    private Vector3 _lastCheckpointPosition;
    private int _lastCheckpointNumber = 0;

    private int _deathCount = 0;
    
    private InputSystem_Actions _inputActions;

    public static event Action<bool> OnPlayerWeaponUnlocked; 

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _inputActions = new InputSystem_Actions();
        
        _inputActions.Enable();
    }

    private void Start()
    {
        ChangeState(GameState.Playing);
        
        if (pauseMenuUI)
            pauseMenuUI.SetActive(false);

        if (player)
            _lastCheckpointPosition = player.transform.position;
        
        _inputActions.Player.Enable();
        _inputActions.UI.Disable();
    }

    public void CheckpointReached(int checkpointNumber, Vector3 checkpointPosition)
    {
        if (checkpointNumber > _lastCheckpointNumber)
        {
            _lastCheckpointNumber = checkpointNumber;
            _lastCheckpointPosition = checkpointPosition;

            if (checkpointNumber >= totalCheckpoints)
            {
                OnFinalCheckpointReached?.Invoke(checkpointPosition);
            }
            else
            {
                OnCheckpointReached?.Invoke(checkpointNumber, checkpointPosition);
            }
        }
    }

    public void OpenEndingPath()
    {
        destroyableTilemap.SetActive(false);
        destroyedObstacles.SetActive(true);
    }

    public void UnlockWeapon()
    {
        OnPlayerWeaponUnlocked?.Invoke(true);
    }

    public void WinGame()
    {
        OnExitReached?.Invoke(_deathCount);
        GameOver();
    }
    
    public void RespawnPlayer()
    {
        if (player && _currentState == GameState.Playing)
        {
            _deathCount++;
            
            CharacterController cc = player.GetComponent<CharacterController>();
            Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();

            if (cc)
            {
                cc.enabled = false;
                player.transform.position = _lastCheckpointPosition;
                cc.enabled = true;
            }
            else if (rb2d)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
                player.transform.position = _lastCheckpointPosition;
            }
            else
            {
                player.transform.position = _lastCheckpointPosition;
            }
        }
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
        
        if (player)
            player.enabled = false;
        
        if (pauseMenuUI)
            pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Playing);

        if (player)
            player.enabled = true;

        if (pauseMenuUI)
            pauseMenuUI.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        
        if (player)
            player.enabled = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void ChangeState(GameState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(newState);
    }
    
    public GameState GetGameState()
    {
        return _currentState;
    }
    
    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions.Dispose();
        }
    }
}