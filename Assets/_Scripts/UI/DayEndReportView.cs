using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayEndReportView : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bankrollText;
    [SerializeField] private TMP_Text expensesText;

    [Header("Button")]
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        gameObject.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void Show(int dayCompleted, float bankroll)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (titleText != null)
            titleText.text = $"Day {dayCompleted} Complete";

        if (bankrollText != null)
            bankrollText.text = $"Bankroll:  ${bankroll:N0}";

        if (expensesText != null)
            expensesText.text = "Daily Expenses:  -$1,500\n(Rent / Salaries / Electricity)";

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