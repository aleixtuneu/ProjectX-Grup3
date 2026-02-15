using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    //Cinematic,
    Playing,
    Paused,
    //BossBattle,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Configuration")]
    [SerializeField] private int totalCheckpoints = 5;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;

    // Game State
    private GameState _currentState = GameState.Playing;
    
    // Score System
    private int _currentScore = 0;
    
    // Lives System
    private int _livesRemaining = 0;
    
    // Checkpoint System
    private Vector3 _lastCheckpointPosition;
    private int _lastCheckpointNumber = 0;
    
    // Input System
    private InputSystem_Actions _inputActions;

    #region Events
    
    // State Events
    public static event Action<GameState> OnStateChanged;
    
    // Score Events
    public static event Action<int> OnScoreChanged;
    
    // Lives Events
    public static event Action<int> OnLivesChanged;
    public static event Action<Vector3, int> OnPlayerRespawn; // position, livesRemaining
    
    // Checkpoint Events
    public static event Action<int, Vector3> OnCheckpointReached;
    
    // Game Events
    public static event Action<int> OnGameOver; // finalScore
    public static event Action<int, int> OnExitReached; // finalScore, deathCount
    public static event Action<bool> OnPlayerWeaponUnlocked;
    
    // Boss Events (future)
    //public static event Action<bool> OnBossDefeated;
    
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
    }

    private void Start()
    {
        InitializeGame();
        
        _inputActions.Player.Enable();
        _inputActions.UI.Disable();
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions.Dispose();
        }
    }

    #endregion

    #region Game Initialization

    private void InitializeGame()
    {
        _currentScore = 0;
        _livesRemaining = startingLives;
        _lastCheckpointPosition = defaultSpawnPosition;
        _lastCheckpointNumber = 0;
        
        ChangeState(GameState.Playing);
        Time.timeScale = 1f;
        
        OnScoreChanged?.Invoke(_currentScore);
        OnLivesChanged?.Invoke(_livesRemaining);
    }

    public void ResetGame()
    {
        InitializeGame();
    }

    #endregion

    #region Score System

    public void AddScore(int points)
    {
        if (_currentState != GameState.Playing) return;
        
        _currentScore += points;
        OnScoreChanged?.Invoke(_currentScore);
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }

    #endregion

    #region Lives & Death System

    public void PlayerDied()
    {
        if (_currentState != GameState.Playing) return;

        _livesRemaining--;
        OnLivesChanged?.Invoke(_livesRemaining);

        if (_livesRemaining > 0)
        {
            // Respawn player
            OnPlayerRespawn?.Invoke(_lastCheckpointPosition, _livesRemaining);
        }
        else
        {
            // Game Over
            TriggerGameOver();
        }
    }

    public int GetLivesRemaining()
    {
        return _livesRemaining;
    }

    public Vector3 GetRespawnPosition()
    {
        return _lastCheckpointPosition;
    }

    #endregion

    #region Checkpoint System

    public void CheckpointReached(int checkpointNumber, Vector3 checkpointPosition)
    {
        if (checkpointNumber > _lastCheckpointNumber)
        {
            _lastCheckpointNumber = checkpointNumber;
            _lastCheckpointPosition = checkpointPosition;
            
            OnCheckpointReached?.Invoke(checkpointNumber, checkpointPosition);
        }
    }

    #endregion

    #region Weapon System

    public void UnlockWeapon()
    {
        OnPlayerWeaponUnlocked?.Invoke(true);
    }

    #endregion

    #region Game State Management

    public void TogglePause()
    {
        if (_currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (_currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (_currentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
        }
    }

    public void ResumeGame()
    {
        if (_currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
            Time.timeScale = 1f;
            
            _inputActions.Player.Enable();
            _inputActions.UI.Disable();
        }
    }

    private void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        
        int deathCount = startingLives - _livesRemaining;
        OnGameOver?.Invoke(_currentScore);
        
        _inputActions.Player.Disable();
        _inputActions.UI.Enable();
    }

    public void WinGame()
    {
        int deathCount = startingLives - _livesRemaining;
        OnExitReached?.Invoke(_currentScore, deathCount);
        
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        
        _inputActions.Player.Disable();
        _inputActions.UI.Enable();
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

    #endregion

    #region Scene Management

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}