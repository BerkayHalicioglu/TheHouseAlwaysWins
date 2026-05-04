using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Bankroll")]
    [SerializeField] private float startingBankroll = 5000f;
    public float CurrentBankroll { get; private set; }

    public static event System.Action<float> OnBankrollChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentBankroll = startingBankroll;
    }

    public void AddMoney(float amount)
    {
        CurrentBankroll += amount;
        OnBankrollChanged?.Invoke(CurrentBankroll);
        if (CurrentBankroll <= 0) GameManager.Instance.TriggerGameOver();
    }

    public void DeductMoney(float amount)
    {
        AddMoney(-amount);
    }
}