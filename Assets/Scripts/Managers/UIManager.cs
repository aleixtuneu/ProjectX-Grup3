using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private TextMeshProUGUI txtLives;
    
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButtonPause;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI txtGameOverScore;
    [SerializeField] private Button menuButtonGameOver;
    [SerializeField] private Button restartButton;
    
    [Header("Information Messages")]
    [SerializeField] private TextMeshProUGUI txtInformationUI;
    [SerializeField] private string txtCheckpoint = "Checkpoint actualitzat!";
    [SerializeField] private string textWeaponUnlock = "Has trobat una arma!";
    [SerializeField] private float informationDisplayDuration = 2f;
    
    [Header("Winning UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI txtWinMessage;
    [SerializeField] private TextMeshProUGUI txtWinScore;
    [SerializeField] private Button menuButtonWin;
    [SerializeField] private string textWinning = "Has pogut escapar!";

    [Header("Final Dialogue (Typewriter)")]
    [SerializeField] private GameObject finalDialoguePanel;
    [SerializeField] private TextMeshProUGUI finalDialogueText;
    [SerializeField] private string finalDialogueMessage = "MISSION 1 COMPLETE…\nBUT THE WAR HAS ONLY JUST BEGUN.";
    [SerializeField] private float typewriterSpeed = 0.05f;

    private Coroutine _typewriterCoroutine;

    #region Unity Lifecycle

    private void OnEnable()
    {
        // Game State Events
        GameManager.OnStateChanged += HandleStateChanged;
        
        // Score & Lives Events
        GameManager.OnScoreChanged += UpdateScoreUI;
        GameManager.OnLivesChanged += UpdateLivesUI;
        
        // Checkpoint Events
        GameManager.OnCheckpointReached += ShowCheckpointUI;
        
        // Game Events
        GameManager.OnPlayerWeaponUnlocked += ShowWeaponUnlockUI;
        GameManager.OnGameOver += ShowGameOverUI;
        GameManager.OnExitReached += ShowWinningUI;
    }

    private void OnDisable()
    {
        // Game State Events
        GameManager.OnStateChanged -= HandleStateChanged;
        
        // Score & Lives Events
        GameManager.OnScoreChanged -= UpdateScoreUI;
        GameManager.OnLivesChanged -= UpdateLivesUI;
        
        // Checkpoint Events
        GameManager.OnCheckpointReached -= ShowCheckpointUI;
        
        // Game Events
        GameManager.OnPlayerWeaponUnlocked -= ShowWeaponUnlockUI;
        GameManager.OnGameOver -= ShowGameOverUI;
        GameManager.OnExitReached -= ShowWinningUI;
    }

    private void Start()
    {
        InitializeUI();
        SetupButtons();
    }

    private void Update()
    {
        // Fix for Input System when TimeScale = 0
        if (Time.timeScale == 0)
        {
            InputSystem.Update();
        }
    }

    #endregion

    #region Initialization

    private void InitializeUI()
    {
        // Hide all panels
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (finalDialoguePanel) finalDialoguePanel.SetActive(false);
        if (txtInformationUI) txtInformationUI.gameObject.SetActive(false);
        
        // Initialize HUD
        UpdateScoreUI(0);
        UpdateLivesUI(GameManager.Instance ? GameManager.Instance.GetLivesRemaining() : 0);
    }

    private void SetupButtons()
    {
        // Pause Menu Buttons
        if (resumeButton) resumeButton.onClick.AddListener(OnResumeClicked);
        if (menuButtonPause) menuButtonPause.onClick.AddListener(OnMenuClicked);
        
        // Game Over Buttons
        if (menuButtonGameOver) menuButtonGameOver.onClick.AddListener(OnMenuClicked);
        if (restartButton) restartButton.onClick.AddListener(OnRestartClicked);
        
        // Win Buttons
        if (menuButtonWin) menuButtonWin.onClick.AddListener(OnMenuClicked);
    }

    #endregion

    #region HUD Updates

    private void UpdateScoreUI(int score)
    {
        if (txtScore)
        {
            txtScore.text = $"Score: {score}";
        }
    }

    private void UpdateLivesUI(int lives)
    {
        if (txtLives)
        {
            txtLives.text = $"Lives: {lives}";
        }
    }

    #endregion

    #region Game State Handling

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                HideAllPanels();
                break;
                
            case GameState.Paused:
                ShowPauseMenu();
                break;
                
            case GameState.GameOver:
                // Game Over UI is handled by OnGameOver/OnExitReached events
                break;
        }
    }

    private void ShowPauseMenu()
    {
        if (pauseMenuPanel)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    private void HideAllPanels()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    #endregion

    #region Information Messages

    private void ShowCheckpointUI(int checkpointNumber, Vector3 position)
    {
        ShowInformationMessage(txtCheckpoint);
    }

    private void ShowWeaponUnlockUI(bool isUnlocked)
    {
        ShowInformationMessage(textWeaponUnlock);
    }

    private void ShowInformationMessage(string message)
    {
        if (txtInformationUI)
        {
            txtInformationUI.text = message;
            StartCoroutine(DisplayInformationUI(informationDisplayDuration));
        }
    }

    private IEnumerator DisplayInformationUI(float displayDuration)
    {
        if (txtInformationUI)
        {
            txtInformationUI.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(displayDuration);
            txtInformationUI.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Game Over UI

    private void ShowGameOverUI(int finalScore)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (txtGameOverScore)
        {
            txtGameOverScore.text = $"Game Over\n\nFinal Score: {finalScore}";
        }
    }

    #endregion

    #region Winning UI

    private void ShowWinningUI(int finalScore, int deathCount)
    {
        // Show win panel with score and stats
        if (winPanel)
        {
            winPanel.SetActive(true);
        }
        
        if (txtWinScore)
        {
            txtWinScore.text = $"Score: {finalScore}";
        }
        
        if (txtWinMessage)
        {
            txtWinMessage.text = (deathCount > 0) ? 
                $"{textWinning}\nPero t'ha costat {deathCount} intents." : 
                $"{textWinning}\nEnhorabona! T'has mantingut invicte!\n\n...Espera, mai faries trampa, oi? oi?";
        }
        
        // Show typewriter dialogue alongside win panel
        ShowFinalDialogue();
    }

    #endregion

    #region Final Dialogue (Typewriter Effect)

    private void ShowFinalDialogue()
    {
        if (finalDialoguePanel != null && finalDialogueText != null)
        {
            finalDialoguePanel.SetActive(true);
            
            if (_typewriterCoroutine != null)
                StopCoroutine(_typewriterCoroutine);
            
            _typewriterCoroutine = StartCoroutine(TypewriterEffect(finalDialogueMessage));
        }
    }

    private IEnumerator TypewriterEffect(string message)
    {
        finalDialogueText.text = message;
        finalDialogueText.maxVisibleCharacters = 0;

        int totalCharacters = message.Length;

        for (int i = 0; i <= totalCharacters; i++)
        {
            finalDialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }
    }

    public void CloseDialogue()
    {
        if (finalDialoguePanel)
            finalDialoguePanel.SetActive(false);
        
        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);
    }

    #endregion

    #region Button Handlers

    private void OnResumeClicked()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.ResumeGame();
        }
    }
    
    private void OnMenuClicked()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.GoToMenu();
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    #endregion
}