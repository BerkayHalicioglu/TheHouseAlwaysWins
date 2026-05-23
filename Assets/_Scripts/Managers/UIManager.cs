using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Views")]
    [SerializeField] private HUDView hudView;

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
    }

    private void OnDisable()
    {
        EconomyManager.OnBankrollChanged -= HandleBankrollChanged;
        GameManager.OnDayStarted -= HandleDayStarted;
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

    //  Event Handlers 

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

    // Public API 

    public void UpdateHUD(float bankroll, float dayProgress)
    {
        if (hudView == null) return;
        hudView.UpdateBankroll(bankroll);
        hudView.UpdateDayProgress(dayProgress);
    }

    public void ShowGameOverScreen() { }       // Step 5
    public void ShowDayEndReport() { }         // Step 4
    public void ShowCheaterCaughtFeedback() { } // Step 6
    public void ShowWrongAccusationFeedback() { } // Step 6
}