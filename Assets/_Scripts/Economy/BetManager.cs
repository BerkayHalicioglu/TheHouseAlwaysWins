using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    [Header("Bet Settings")]
    [SerializeField] private float minimumBet = 100f;
    [SerializeField] private float maximumBet = 1000f;

    public float CurrentBet { get; private set; }
    public bool HasActiveBet => CurrentBet > 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool PlaceBet(float amount, float currentBankroll)
    {
        if (HasActiveBet)
        {
            Debug.Log("There is already an active bet.");
            return false;
        }

        if (amount < minimumBet)
        {
            Debug.Log("Bet amount is below minimum bet.");
            return false;
        }

        if (amount > maximumBet)
        {
            Debug.Log("Bet amount is above maximum bet.");
            return false;
        }

        if (currentBankroll < amount)
        {
            Debug.Log("Not enough bankroll.");
            return false;
        }

        CurrentBet = amount;
        Debug.Log("Bet placed: " + amount);

        return true;
    }

    public float ResolveBet(BetResult result)
    {
        if (!HasActiveBet)
        {
            Debug.Log("No active bet.");
            return 0f;
        }

        float payout = 0f;

        switch (result)
        {
            case BetResult.Win:
                payout = CurrentBet * 2f;
                break;

            case BetResult.Lose:
                payout = 0f;
                break;

            case BetResult.Push:
                payout = CurrentBet;
                break;

            case BetResult.Blackjack:
                payout = CurrentBet * 2.5f;
                break;
        }

        Debug.Log("Bet resolved: " + result + " | Payout: " + payout);

        CurrentBet = 0f;
        return payout;
    }

    public float CancelBet()
    {
        if (!HasActiveBet)
        {
            Debug.Log("No active bet to cancel.");
            return 0f;
        }

        float refund = CurrentBet;
        CurrentBet = 0f;

        Debug.Log("Bet cancelled. Refund: " + refund);
        return refund;
    }

    public void ResolveNpcGame(float betAmount, BetResult result)
    {
        if (betAmount <= 0f)
        {
            Debug.LogWarning("NPC bet amount must be greater than zero.");
            return;
        }

        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager not found.");
            return;
        }

        switch (result)
        {
            case BetResult.Win:
                EconomyManager.Instance.DeductMoney(betAmount);
                break;

            case BetResult.Lose:
                EconomyManager.Instance.AddMoney(betAmount);
                break;

            case BetResult.Push:
                break;

            case BetResult.Blackjack:
                EconomyManager.Instance.DeductMoney(betAmount * 1.5f);
                break;
        }

        Debug.Log("NPC game resolved: " + result + " | Bet: " + betAmount);
    }
}
