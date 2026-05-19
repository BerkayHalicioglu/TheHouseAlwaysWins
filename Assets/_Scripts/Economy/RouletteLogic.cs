using UnityEngine;

public enum RouletteBetType
{
    Number,
    Red,
    Black,
    Even,
    Odd
}

public class RouletteLogic : MonoBehaviour
{
    public BetResult DetermineResult(RouletteBetType betType, int selectedNumber, int winningNumber)
    {
        if (winningNumber < 0 || winningNumber > 36)
        {
            Debug.LogError("Invalid roulette winning number.");
            return BetResult.Lose;
        }

        switch (betType)
        {
            case RouletteBetType.Number:
                return selectedNumber == winningNumber ? BetResult.Win : BetResult.Lose;

            case RouletteBetType.Red:
                return IsRed(winningNumber) ? BetResult.Win : BetResult.Lose;

            case RouletteBetType.Black:
                return IsBlack(winningNumber) ? BetResult.Win : BetResult.Lose;

            case RouletteBetType.Even:
                return winningNumber != 0 && winningNumber % 2 == 0 ? BetResult.Win : BetResult.Lose;

            case RouletteBetType.Odd:
                return winningNumber != 0 && winningNumber % 2 != 0 ? BetResult.Win : BetResult.Lose;

            default:
                return BetResult.Lose;
        }
    }

    public float GetPayoutMultiplier(RouletteBetType betType)
    {
        switch (betType)
        {
            case RouletteBetType.Number:
                return 36f;

            case RouletteBetType.Red:
            case RouletteBetType.Black:
            case RouletteBetType.Even:
            case RouletteBetType.Odd:
                return 2f;

            default:
                return 0f;
        }
    }

    private bool IsRed(int number)
    {
        return number == 1 || number == 3 || number == 5 || number == 7 || number == 9 ||
               number == 12 || number == 14 || number == 16 || number == 18 ||
               number == 19 || number == 21 || number == 23 || number == 25 || number == 27 ||
               number == 30 || number == 32 || number == 34 || number == 36;
    }

    private bool IsBlack(int number)
    {
        return number != 0 && !IsRed(number);
    }
}
