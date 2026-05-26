using UnityEngine;

public enum GameTableType
{
    Blackjack,
    Roulette
}

public class GameTableInteraction : MonoBehaviour, ICardDealable
{
    [Header("Table Settings")]
    [SerializeField] private string tableName = "Game Table";
    [SerializeField] private GameTableType tableType;
    [SerializeField] private BlackjackLogic blackjackLogic;
    [SerializeField] private BetManager betManager;

    private void Awake()
    {
        if (blackjackLogic == null)
            blackjackLogic = FindFirstObjectByType<BlackjackLogic>();

        if (betManager == null)
            betManager = FindFirstObjectByType<BetManager>();
    }

    public void DealCards()
    {
        switch (tableType)
        {
            case GameTableType.Blackjack:
                StartBlackjackRound();
                break;

            case GameTableType.Roulette:
                StartRouletteRound();
                break;
        }
    }

    private void StartBlackjackRound()
    {
        int dealerCard1 = DrawCard();
        int dealerCard2 = DrawCard();

        int playerCard1 = DrawCard();
        int playerCard2 = DrawCard();

        int dealerTotal = dealerCard1 + dealerCard2;
        int playerTotal = playerCard1 + playerCard2;
        bool playerHasBlackjack = playerTotal == 21;

        Debug.Log($"[{tableName}] Blackjack Round Started");
        Debug.Log($"[BLACKJACK] Dealer Cards -> {dealerCard1}, {dealerCard2} | Total: {dealerTotal}");
        Debug.Log($"[BLACKJACK] Player Cards -> {playerCard1}, {playerCard2} | Total: {playerTotal}");
        Debug.Log("[UI HOOK] Show blackjack dealing animation");
        Debug.Log("[UI HOOK] Update blackjack card visuals");
        Debug.Log("[UI HOOK] Display blackjack totals");

        if (blackjackLogic != null && betManager != null)
        {
            BetResult result = blackjackLogic.DetermineResult(playerTotal, dealerTotal, playerHasBlackjack);
            float payout = betManager.ResolveBet(result);

            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddMoney(payout);
            }

            Debug.Log($"[BLACKJACK RESULT] {result} | Payout: {payout}");
        }

    }

    private void StartRouletteRound()
    {
        int resultNumber = Random.Range(0, 37);

        string color;

        if (resultNumber == 0)
        {
            color = "Green";
        }
        else if (resultNumber % 2 == 0)
        {
            color = "Black";
        }
        else
        {
            color = "Red";
        }

        Debug.Log($"[{tableName}] Roulette Round Started");
        Debug.Log($"[ROULETTE] Result -> {resultNumber} / {color}");
        Debug.Log("[UI HOOK] Start roulette spin animation");
        Debug.Log("[UI HOOK] Display roulette result");

    }

    private int DrawCard()
    {
        return Random.Range(1, 11);
    }
}