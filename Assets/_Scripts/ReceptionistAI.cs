using UnityEngine;
using TMPro;
using System.Collections;

public class ReceptionistAI : MonoBehaviour
{
    private Animator _animator;
    private static readonly int TriggerReaction = Animator.StringToHash("isTriggered");

    [Header("UI Ayarlarý")]
    [SerializeField] private TextMeshProUGUI speechBubbleText;
    [SerializeField] private GameObject bubbleContainer;
    [SerializeField] private float displayTime = 4f;

    private Coroutine _currentBubbleCoroutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
    }

    private void OnEnable()
    {
        CustomerAI.OnCheaterCaught += HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom += HandleInnocent;
        CustomerAI.OnCheaterMissed += HandleMissed;

        QTEManager.OnQTEFailed += HandleQTEFailed;
    }

    private void OnDisable()
    {
        CustomerAI.OnCheaterCaught -= HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom -= HandleInnocent;
        CustomerAI.OnCheaterMissed -= HandleMissed;

        QTEManager.OnQTEFailed -= HandleQTEFailed;

    }

    private void HandleCheaterCaught(float amount)
    {
        TriggerReactionAndSpeech("Harika iþ! Bir hileciyi daha paketledik.");
    }

    private void HandleInnocent(float amount)
    {
        TriggerReactionAndSpeech("Eyvah... Masum birine mi bulaþtýk? Yönetim çýldýracak!");
    }

    private void HandleMissed(float amount)
    {
        TriggerReactionAndSpeech("Nasýl kaçýrýrsýn?! Gözünün önündeydi!");
    }


    private void HandleQTEFailed()
    {
        TriggerReactionAndSpeech("Tüh, elimizden kaçtý! Biraz daha hýzlý olmalýydýn.");
    }

    private void TriggerReactionAndSpeech(string message)
    {
        if (_animator != null) _animator.SetBool(TriggerReaction, true);

        if (_currentBubbleCoroutine != null) StopCoroutine(_currentBubbleCoroutine);

        _currentBubbleCoroutine = StartCoroutine(ShowSpeechBubble(message));
    }

    private IEnumerator ShowSpeechBubble(string message)
    {
        if (speechBubbleText != null) speechBubbleText.text = message;
        if (bubbleContainer != null) bubbleContainer.SetActive(true);

        yield return new WaitForSeconds(2f);
        if (_animator != null) _animator.SetBool(TriggerReaction, false);

        yield return new WaitForSeconds(displayTime - 2f);

        if (bubbleContainer != null) bubbleContainer.SetActive(false);
    }
}