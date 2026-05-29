using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager Instance { get; private set; }

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float sprintDrainPerSecond = 15f;
    [SerializeField] private float wrongGuessPenalty = 25f;
    [SerializeField] private float cheaterCaughtReward = 20f;
    [SerializeField] private float correctReleaseReward = 10f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool CanSprint => CurrentStamina > 0f;
    public float SprintDrainPerSecond => sprintDrainPerSecond;

    public static event System.Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentStamina = maxStamina;
        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }

    private void OnEnable()
    {
        CustomerAI.OnInnocentSentToBackRoom += HandleWrongGuess;
        CustomerAI.OnCheaterMissed += HandleWrongGuess;
        CustomerAI.OnCheaterCaught += HandleCheaterCaught;
        CustomerAI.OnCorrectRelease += HandleCorrectRelease;
        GameManager.OnDayStarted += HandleDayStarted;
    }

    private void OnDisable()
    {
        CustomerAI.OnInnocentSentToBackRoom -= HandleWrongGuess;
        CustomerAI.OnCheaterMissed -= HandleWrongGuess;
        CustomerAI.OnCheaterCaught -= HandleCheaterCaught;
        CustomerAI.OnCorrectRelease -= HandleCorrectRelease;
        GameManager.OnDayStarted -= HandleDayStarted;
    }

    private void HandleWrongGuess(float _)
    {
        DrainStamina(wrongGuessPenalty);
    }

    private void HandleCheaterCaught(float _)
    {
        RefillStamina(cheaterCaughtReward);
    }

    private void HandleCorrectRelease(float _)
    {
        RefillStamina(correctReleaseReward);
    }

    private void HandleDayStarted()
    {
        CurrentStamina = maxStamina;
        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }

    public void DrainStamina(float amount)
    {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }

    public void RefillStamina(float amount)
    {
        CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + amount);
        OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }
}
