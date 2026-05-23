using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Views")]
    [SerializeField] private HUDView hudView;
    [SerializeField] private InteractionPromptView interactionPromptView;
    [SerializeField] private QTEView qteView;
    [SerializeField] private DayEndReportView dayEndReportView;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EconomyManager.OnBankrollChanged += HandleBankrollChanged;
        GameManager.OnDayStarted += HandleDayStarted;
        GameManager.OnDayEnded += HandleDayEnded;
        PlayerInteractor.OnShowPrompt += HandleShowPrompt;
        PlayerInteractor.OnHidePrompt += HandleHidePrompt;
    }

    private void OnDisable()
    {
        EconomyManager.OnBankrollChanged -= HandleBankrollChanged;
        GameManager.OnDayStarted -= HandleDayStarted;
        GameManager.OnDayEnded -= HandleDayEnded;
        PlayerInteractor.OnShowPrompt -= HandleShowPrompt;
        PlayerInteractor.OnHidePrompt -= HandleHidePrompt;
    }

    private void Start()
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

        int dayCompleted = GameManager.Instance.CurrentDay - 1;
        float bankroll = EconomyManager.Instance.CurrentBankroll;

        dayEndReportView.Show(dayCompleted, bankroll);
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

    public void ShowDayEndReport()
    {
        HandleDayEnded();
    }

    public void ShowGameOverScreen() { }           
    public void ShowCheaterCaughtFeedback() { }    
    public void ShowWrongAccusationFeedback() { }  
}