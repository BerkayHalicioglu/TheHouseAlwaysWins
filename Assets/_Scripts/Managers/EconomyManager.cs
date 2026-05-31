using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Bankroll")]
    [SerializeField] private float startingBankroll = 5000f;
    public float CurrentBankroll { get; private set; }
    public float StartingBankroll => startingBankroll;

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
        OnBankrollChanged?.Invoke(CurrentBankroll);
    }

    public void Initialize(float bankroll)
    {
        CurrentBankroll = bankroll;
        OnBankrollChanged?.Invoke(CurrentBankroll);
    }

    public void AddMoney(float amount)
    {
        if (amount <= 0f) return;

        CurrentBankroll += amount;
        OnBankrollChanged?.Invoke(CurrentBankroll);
    }

    public bool DeductMoney(float amount)
    {
        if (amount <= 0f) return false;

        if (CurrentBankroll >= amount)
        {
            CurrentBankroll -= amount;
            OnBankrollChanged?.Invoke(CurrentBankroll);

            if (CurrentBankroll <= 0f)
            {
                GameManager.Instance.TriggerGameOver();
            }

            return true;
        }

        GameManager.Instance.TriggerGameOver();
        return false;
    }
}
