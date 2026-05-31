using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QTEView : MonoBehaviour
{
    [Header("Prompt Panel")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text keyPromptText;
    [SerializeField] private Image timerFill;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;

    [Header("Settings")]
    [SerializeField] private float resultDisplayDuration = 1.2f;

    private Key[] sequence;
    private int   currentStep;
    private float timeLimit;
    private float timeRemaining;
    private bool  isActive;

    private void Awake()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        QTEManager.OnQTEStarted      += HandleQTEStarted;
        QTEManager.OnQTEStepAdvanced += HandleQTEStepAdvanced;
        QTEManager.OnQTESuccess      += HandleQTESuccess;
        QTEManager.OnQTEFailed       += HandleQTEFailed;
    }

    private void OnDisable()
    {
        QTEManager.OnQTEStarted      -= HandleQTEStarted;
        QTEManager.OnQTEStepAdvanced -= HandleQTEStepAdvanced;
        QTEManager.OnQTESuccess      -= HandleQTESuccess;
        QTEManager.OnQTEFailed       -= HandleQTEFailed;
    }

    private void Update()
    {
        if (!isActive) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining  = Mathf.Max(timeRemaining, 0f);

        if (timerFill != null)
            timerFill.fillAmount = timeRemaining / timeLimit;
    }

    private void HandleQTEStarted(Key[] seq, float duration)
    {
        StopAllCoroutines();

        sequence      = seq;
        currentStep   = 0;
        timeLimit     = duration;
        timeRemaining = duration;
        isActive      = true;

        if (timerFill != null)  timerFill.fillAmount = 1f;
        if (resultPanel != null) resultPanel.SetActive(false);
        if (promptPanel != null) promptPanel.SetActive(true);

        RefreshKeyDisplay();
    }

    private void HandleQTEStepAdvanced(int step)
    {
        currentStep = step;
        RefreshKeyDisplay();
    }

    private void HandleQTESuccess()
    {
        isActive = false;
        if (promptPanel != null) promptPanel.SetActive(false);
        ShowResult("SUCCESS!", Color.green);
    }

    private void HandleQTEFailed()
    {
        isActive = false;
        if (promptPanel != null) promptPanel.SetActive(false);
        ShowResult("FAILED!", Color.red);
    }

    // Builds e.g. "✓  [ J ]  K" — done / current / upcoming
    private void RefreshKeyDisplay()
    {
        if (keyPromptText == null || sequence == null) return;

        var sb = new StringBuilder();
        for (int i = 0; i < sequence.Length; i++)
        {
            if (i > 0) sb.Append("   ");

            if (i < currentStep)
                sb.Append("✓");
            else if (i == currentStep)
                sb.Append($"[ {sequence[i]} ]");
            else
                sb.Append(sequence[i].ToString());
        }
        keyPromptText.text = sb.ToString();
    }

    private void ShowResult(string message, Color color)
    {
        if (resultText != null)
        {
            resultText.text  = message;
            resultText.color = color;
        }

        if (resultPanel != null) resultPanel.SetActive(true);
        StartCoroutine(HideResultAfterDelay());
    }

    private IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
