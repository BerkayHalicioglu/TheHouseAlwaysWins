using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Views")]
    [SerializeField] private HUDView hudView;
    [SerializeField] private InteractionPromptView interactionPromptView;
    [SerializeField] private QTEView qteView;

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
        PlayerInteractor.OnShowPrompt += HandleShowPrompt;
        PlayerInteractor.OnHidePrompt += HandleHidePrompt;
    }

    private void OnDisable()
    {
        EconomyManager.OnBankrollChanged -= HandleBankrollChanged;
        GameManager.OnDayStarted -= HandleDayStarted;
        PlayerInteractor.OnShowPrompt -= HandleShowPrompt;
        PlayerInteractor.OnHidePrompt -= HandleHidePrompt;
    }

    private void Update()
    {
        UpdateDayProgressBar();
    }


    private void HandleBankrollChanged(float newAmount)
    {
        if (hudView == null) return;
        hudView.UpdateBankroll(newAmount);
    }

    private void HandleDayStarted()
    {
        if (hudView == null) return;
        hudView.UpdateDay(GameManager.Instance.CurrentDay);
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
        if (interactionPromptView == null) return;
        interactionPromptView.ShowPrompt(message);
    }

    private void HandleHidePrompt()
    {
        if (interactionPromptView == null) return;
        interactionPromptView.HidePrompt();
    }



    public void UpdateHUD(float bankroll, float dayProgress)
    {
        if (hudView == null) return;
        hudView.UpdateBankroll(bankroll);
        hudView.UpdateDayProgress(dayProgress);
    }

    public void ShowGameOverScreen() { }          
    public void ShowDayEndReport() { }            
    public void ShowCheaterCaughtFeedback() { }   
    public void ShowWrongAccusationFeedback() { }  
}