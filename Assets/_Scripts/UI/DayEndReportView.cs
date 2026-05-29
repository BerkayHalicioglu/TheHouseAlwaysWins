using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayEndReportView : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text titleText;

    [Header("Interrogation Stats")]
    [SerializeField] private TMP_Text cheatersCaughtText;
    [SerializeField] private TMP_Text innocentsSentText;
    [SerializeField] private TMP_Text cheatersMissedText;
    [SerializeField] private TMP_Text correctReleasesText;

    [Header("Finance")]
    [SerializeField] private TMP_Text bankrollText;
    [SerializeField] private TMP_Text expensesText;
    [SerializeField] private TMP_Text netProfitText;

    [Header("Efficiency")]
    [SerializeField] private TMP_Text efficiencyPercentText;
    [SerializeField] private TMP_Text efficiencyRatingText;

    [Header("Button")]
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void Show(int dayCompleted, DayStats stats)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (titleText != null)
            titleText.text = $"Day {dayCompleted} Complete";

        // Interrogation stats
        if (cheatersCaughtText != null)
            cheatersCaughtText.text  = $"Cheaters Caught:   {stats.cheatersCaught}";

        if (innocentsSentText != null)
            innocentsSentText.text   = $"Innocents Accused: {stats.innocentsSent}";

        if (cheatersMissedText != null)
            cheatersMissedText.text  = $"Cheaters Missed:   {stats.cheatersMissed}";

        if (correctReleasesText != null)
            correctReleasesText.text = $"Correct Releases:  {stats.correctReleases}";

        // Finance
        float multiplier = DifficultyManager.Instance != null ? DifficultyManager.Instance.ExpenseMultiplier : 1f;
        float expenses = 1500f * multiplier;

        if (bankrollText != null)
            bankrollText.text  = $"Bankroll:  ${stats.bankrollEnd:N0}";

        if (expensesText != null)
            expensesText.text  = $"Expenses:  -${expenses:N0}";

        if (netProfitText != null)
        {
            string sign  = stats.NetProfit >= 0 ? "+" : "";
            string color = stats.NetProfit >= 0 ? "#00FF88" : "#FF4444";
            netProfitText.text = $"Net:  <color={color}>{sign}${stats.NetProfit:N0}</color>";
        }

        // Efficiency
        if (efficiencyPercentText != null)
            efficiencyPercentText.text = stats.TotalDecisions > 0
                ? $"Accuracy:  %{stats.EfficiencyPercent:F0}"
                : "Accuracy:  —";

        if (efficiencyRatingText != null)
            efficiencyRatingText.text = stats.TotalDecisions > 0
                ? $"Rating:  <color={stats.EfficiencyRatingColor}>{stats.EfficiencyRating}</color>"
                : "Rating:  —";

        gameObject.SetActive(true);
    }

    private void OnContinueClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
        GameManager.Instance.StartDay();
    }
}
