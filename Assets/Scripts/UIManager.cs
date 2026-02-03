using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button menuButtonPause;
    
    [Header("Checkpoint UI")]
    [SerializeField] private TextMeshProUGUI txtInformationUI;
    [SerializeField] private string txtCheckpoint = "Checkpoint actualitzat!";
    [SerializeField] private float checkpointDisplayDuration = 2f;
    
    [Header("WeaponUnlock UI")] 
    [SerializeField] private string textWeaponUnlock = "Has trobat una arma!";

    [Header("Final Dialogue")]
    [SerializeField] private GameObject finalDialoguePanel;
    [SerializeField] private TextMeshProUGUI finalDialogueText;
    [SerializeField] private string finalDialogueMessage = "Oh no! No puc seguir per aquí! He de tornar enrere!";
    [SerializeField] private float typewriterSpeed = 0.05f;
    
    [Header("Winning UI")]
    [SerializeField] private string textWinning = "Has pogut escapar!";

    private Coroutine _typewriterCoroutine;

    private void OnEnable()
    {
        GameManager.OnCheckpointReached += ShowCheckpointUI;
        GameManager.OnFinalCheckpointReached += StartEndGame;
        GameManager.OnPlayerWeaponUnlocked += ShowWeaponUnlockUI;
        GameManager.OnExitReached += ShowWinningUI;
    }

    private void OnDisable()
    {
        GameManager.OnCheckpointReached -= ShowCheckpointUI;
        GameManager.OnFinalCheckpointReached -= StartEndGame;
        GameManager.OnPlayerWeaponUnlocked -= ShowWeaponUnlockUI;
        GameManager.OnExitReached -= ShowWinningUI;
    }

    private void Start()
    {
        if (txtInformationUI)
            txtInformationUI.gameObject.SetActive(false);
        
        if (finalDialoguePanel)
            finalDialoguePanel.SetActive(false);
        
        resumeButton.onClick.AddListener(OnResumeClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
        menuButtonPause.onClick.AddListener(OnMenuClicked);
    }

    private void Update()
    {
        if(Time.timeScale == 0)
        {
            InputSystem.Update();
        }
    }

    private void OnResumeClicked()
    {
        GameManager.Instance.ResumeGame();
    }
    
    private void OnMenuClicked()
    {
        GameManager.Instance.GoToMenu();
    }

    private void ShowCheckpointUI(int checkpointNumber, Vector3 a )
    {
        if (txtInformationUI)
        {
            txtInformationUI.text = txtCheckpoint;
            StartCoroutine(DisplayInformationUI(checkpointDisplayDuration));
        }
    }

    private IEnumerator DisplayInformationUI(float displayDuration)
    {
        txtInformationUI.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(displayDuration);
        txtInformationUI.gameObject.SetActive(false);
    }
    
    private void ShowWeaponUnlockUI(bool isUnlocked)
    {
        if (txtInformationUI)
        {
            txtInformationUI.text = textWeaponUnlock;
            StartCoroutine(DisplayInformationUI(checkpointDisplayDuration));
        }
    }
    
    private void ShowWinningUI(int playerDeathCount)
    {
        if (menuButton)
        {
            Debug.Log("Activating Winning UI menubutton");
            menuButton.gameObject.SetActive(true);
        }
        
        // Debug.Log($"{playerDeathCount}: has guanyat");
        if (txtInformationUI)
        {
            txtInformationUI.text = (playerDeathCount > 0) ? 
                $"{textWinning} \nPero t'ha costat {playerDeathCount} intents." : 
                    $"{textWinning} \n Enhorabona! T'has mantingut invicte! \n\n...Espera, mai faries trampa, oi? oi?" ;
            txtInformationUI.gameObject.SetActive(true);
        }
    }

    private void StartEndGame(Vector3 a)
    {
        ShowFinalDialogue();
        GameManager.Instance.OpenEndingPath();
    }
    
    // The event receives a V3 and passes it over, but it's not needed here
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
}