using UnityEngine;

public class BlackjackLogic : MonoBehaviour
{
    public BetResult DetermineResult(int playerScore, int dealerScore, bool playerHasBlackjack)
    {
        if (playerHasBlackjack)
            return BetResult.Blackjack;

        if (playerScore > 21)
            return BetResult.Lose;

        if (dealerScore > 21)
            return BetResult.Win;

        if (playerScore > dealerScore)
            return BetResult.Win;

        if (playerScore < dealerScore)
            return BetResult.Lose;

        return BetResult.Push;
    }

    public int CalculateCardValue(int cardNumber)
    {
        if (cardNumber >= 10)
            return 10;

        if (cardNumber == 1)
            return 11;

        return cardNumber;
    }
}
