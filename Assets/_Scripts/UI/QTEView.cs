using System.Collections;
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

    private float timeLimit;
    private float timeRemaining;
    private bool isActive;

    private void Awake()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        QTEManager.OnQTEStarted += HandleQTEStarted;
        QTEManager.OnQTESuccess += HandleQTESuccess;
        QTEManager.OnQTEFailed += HandleQTEFailed;
    }

    private void OnDisable()
    {
        QTEManager.OnQTEStarted -= HandleQTEStarted;
        QTEManager.OnQTESuccess -= HandleQTESuccess;
        QTEManager.OnQTEFailed -= HandleQTEFailed;
    }

    private void Update()
    {
        if (!isActive) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(timeRemaining, 0f);

        if (timerFill != null)
            timerFill.fillAmount = timeRemaining / timeLimit;
    }

    private void HandleQTEStarted(Key key, float duration)
    {
        StopAllCoroutines();

        timeLimit = duration;
        timeRemaining = duration;
        isActive = true;

        if (keyPromptText != null)
            keyPromptText.text = $"Press  {key.ToString().ToUpper()}";

        if (timerFill != null)
            timerFill.fillAmount = 1f;

        if (resultPanel != null) resultPanel.SetActive(false);
        if (promptPanel != null) promptPanel.SetActive(true);
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

    private void ShowResult(string message, Color color)
    {
        if (resultText != null)
        {
            resultText.text = message;
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