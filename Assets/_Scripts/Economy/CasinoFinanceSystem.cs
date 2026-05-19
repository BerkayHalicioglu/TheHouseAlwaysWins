using UnityEngine;

public class CasinoFinanceSystem : MonoBehaviour
{
    [Header("Daily Expenses")]
    [SerializeField] private float dailyRent = 500f;
    [SerializeField] private float dailySalary = 750f;
    [SerializeField] private float dailyElectricity = 250f;

    private void OnEnable()
    {
        GameManager.OnDayEnded += ApplyDailyExpenses;
    }

    private void OnDisable()
    {
        GameManager.OnDayEnded -= ApplyDailyExpenses;
    }

    public void ApplyDailyExpenses()
    {
        float totalExpense = dailyRent + dailySalary + dailyElectricity;

        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager not found.");
            return;
        }

        bool expensePaid = EconomyManager.Instance.DeductMoney(totalExpense);

        if (!expensePaid)
        {
            Debug.LogWarning("Daily expenses could not be paid: " + totalExpense);
            return;
        }

        Debug.Log("Daily expenses applied: " + totalExpense);
    }

    public void PayCompensation(float amount)
    {
        if (amount <= 0f)
            return;

        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager not found.");
            return;
        }

        bool compensationPaid = EconomyManager.Instance.DeductMoney(amount);

        if (!compensationPaid)
        {
            Debug.LogWarning("Compensation could not be paid: " + amount);
            return;
        }

        Debug.Log("Compensation paid: " + amount);
    }
}
