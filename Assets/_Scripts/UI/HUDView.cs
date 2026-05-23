using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDView : MonoBehaviour
{
    [Header("Bankroll")]
    [SerializeField] private TMP_Text bankrollText;

    [Header("Day Info")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private Image dayProgressFill;

    public void UpdateBankroll(float amount)
    {
        if (bankrollText == null) return;
        bankrollText.text = $"${amount:N0}";
    }

    public void UpdateDay(int day)
    {
        if (dayText == null) return;
        dayText.text = $"Day {day}";
    }

    public void UpdateDayProgress(float progress)
    {
        if (dayProgressFill == null) return;
        dayProgressFill.fillAmount = Mathf.Clamp01(progress);
    }
}