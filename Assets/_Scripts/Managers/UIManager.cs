using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Views")]
    [SerializeField] private HUDView hudView;
    [SerializeField] private InteractionPromptView interactionPromptView;
    [SerializeField] private QTEView qteView;
    [SerializeField] private InterrogationView interrogationView;
    [SerializeField] private DayEndReportView dayEndReportView;
    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private FeedbackView feedbackView;
    [SerializeField] private PauseMenuView pauseMenuView;
    [SerializeField] private SettingsView settingsView;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EconomyManager.OnBankrollChanged += HandleBankrollChanged;
        GameManager.OnDayStarted += HandleDayStarted;
        GameManager.OnDayEnded += HandleDayEnded;
        GameManager.OnGameOver += HandleGameOver;
        PlayerInteractor.OnShowPrompt += HandleShowPrompt;
        PlayerInteractor.OnHidePrompt += HandleHidePrompt;
        CustomerAI.OnCheaterCaught += HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom += HandleInnocentSentToBackRoom;
        CustomerAI.OnCheaterMissed += HandleCheaterMissed;
        CustomerAI.OnCorrectRelease += HandleCorrectRelease;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EconomyManager.OnBankrollChanged -= HandleBankrollChanged;
        GameManager.OnDayStarted -= HandleDayStarted;
        GameManager.OnDayEnded -= HandleDayEnded;
        GameManager.OnGameOver -= HandleGameOver;
        PlayerInteractor.OnShowPrompt -= HandleShowPrompt;
        PlayerInteractor.OnHidePrompt -= HandleHidePrompt;
        CustomerAI.OnCheaterCaught -= HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom -= HandleInnocentSentToBackRoom;
        CustomerAI.OnCheaterMissed -= HandleCheaterMissed;
        CustomerAI.OnCorrectRelease -= HandleCorrectRelease;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CasinoFloor") return;

        hudView = Object.FindFirstObjectByType<HUDView>(FindObjectsInactive.Include);
        interactionPromptView = Object.FindFirstObjectByType<InteractionPromptView>(FindObjectsInactive.Include);
        qteView = Object.FindFirstObjectByType<QTEView>(FindObjectsInactive.Include);
        dayEndReportView = Object.FindFirstObjectByType<DayEndReportView>(FindObjectsInactive.Include);
        gameOverView = Object.FindFirstObjectByType<GameOverView>(FindObjectsInactive.Include);
        feedbackView = Object.FindFirstObjectByType<FeedbackView>(FindObjectsInactive.Include);
        interrogationView = Object.FindFirstObjectByType<InterrogationView>(FindObjectsInactive.Include);
        pauseMenuView = Object.FindFirstObjectByType<PauseMenuView>(FindObjectsInactive.Include);
        settingsView = Object.FindFirstObjectByType<SettingsView>(FindObjectsInactive.Include);

        InitializeHUD();
    }

    private void InitializeHUD()
    {
        if (hudView == null) return;

        if (EconomyManager.Instance != null)
            hudView.UpdateBankroll(EconomyManager.Instance.CurrentBankroll);

        if (GameManager.Instance != null)
            hudView.UpdateDay(GameManager.Instance.CurrentDay);

        hudView.UpdateDayProgress(0f);
    }

    private void Update()
    {
        UpdateDayProgressBar();
        HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        if (Keyboard.current == null) return;
        if (pauseMenuView == null) return;
        if (!Keyboard.current.tabKey.wasPressedThisFrame) return;

        if (pauseMenuView.IsPaused)
            pauseMenuView.Resume();
        else
            pauseMenuView.Pause();
    }


    private void HandleBankrollChanged(float newAmount)
    {
        if (hudView != null)
            hudView.UpdateBankroll(newAmount);
    }

    private void HandleDayStarted()
    {
        if (hudView != null)
            hudView.UpdateDay(GameManager.Instance.CurrentDay);
    }

    private void HandleDayEnded()
    {
        StartCoroutine(ShowDayEndReportNextFrame());
    }

    private IEnumerator ShowDayEndReportNextFrame()
    {
        yield return null;

        if (dayEndReportView == null) yield break;
        if (GameManager.Instance.CurrentState == GameState.GameOver) yield break;

        int dayCompleted = GameManager.Instance.CurrentDay - 1;
        float bankroll = EconomyManager.Instance.CurrentBankroll;

        dayEndReportView.Show(dayCompleted, bankroll);
    }

    private void HandleGameOver()
    {
        if (dayEndReportView != null)
            dayEndReportView.gameObject.SetActive(false);

        if (gameOverView == null) return;

        int daysSurvived = GameManager.Instance.CurrentDay - 1;
        float finalBankroll = EconomyManager.Instance.CurrentBankroll;

        gameOverView.Show(daysSurvived, finalBankroll);
    }

    private void HandleCheaterCaught(float amount)
    {
        if (feedbackView != null)
            feedbackView.ShowCheaterCaught(amount);
    }

    private void HandleInnocentSentToBackRoom(float amount)
    {
        if (feedbackView != null)
            feedbackView.ShowInnocentCompensation(amount);
    }

    private void HandleCheaterMissed(float amount)
    {
        if (feedbackView != null)
            feedbackView.ShowCheaterMissed(amount);
    }

    private void HandleCorrectRelease(float amount)
    {
        if (feedbackView != null)
            feedbackView.ShowCorrectRelease(amount);
    }

    private void UpdateDayProgressBar()
    {
        if (hudView == null) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.CasinoFloor) return;

        hudView.UpdateDayProgress(GameManager.Instance.GetDayProgress());
    }

    private void HandleShowPrompt(string message)
    {
        if (interactionPromptView != null)
            interactionPromptView.ShowPrompt(message);
    }

    private void HandleHidePrompt()
    {
        if (interactionPromptView != null)
            interactionPromptView.HidePrompt();
    }


    public void UpdateHUD(float bankroll, float dayProgress)
    {
        if (hudView == null) return;
        hudView.UpdateBankroll(bankroll);
        hudView.UpdateDayProgress(dayProgress);
    }

    public void ShowDayEndReport() { HandleDayEnded(); }
    public void ShowGameOverScreen() { HandleGameOver(); }

    public void ShowCheaterCaughtFeedback()
    {
        if (feedbackView != null)
            feedbackView.ShowCheaterCaught(500f);
    }

    public void ShowWrongAccusationFeedback()
    {
        if (feedbackView != null)
            feedbackView.ShowWrongAccusation(250f);
    }
}