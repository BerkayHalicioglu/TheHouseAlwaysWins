using System.Collections;
using UnityEngine;
using TMPro;

public class FeedbackView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text amountText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 2.5f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowCheaterCaught(float amount)    => Show("CHEATER CAUGHT!",    $"+${amount:N0}",               Color.green,                successSound);
    public void ShowInnocentCompensation(float amount) => Show("THEY WERE INNOCENT!", $"-${amount:N0} Compensation", Color.red,                  errorSound);
    public void ShowCorrectRelease(float amount)   => Show("CORRECT CALL!",      $"+${amount:N0}",               Color.green,                successSound);
    public void ShowCheaterMissed(float amount)    => Show("CHEATER ESCAPED!",   $"-${amount:N0}",               new Color(1f, 0.5f, 0f),    errorSound);
    public void ShowWrongAccusation(float amount)  => Show("WRONG ACCUSATION!",  $"-${amount:N0}",               Color.red,                  errorSound);

    private void Show(string title, string amount, Color color, AudioClip clip)
    {
        if (titleText != null)  { titleText.text  = title;  titleText.color  = color; }
        if (amountText != null) { amountText.text = amount; amountText.color = color; }

        PlaySound(clip);

        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(displayDuration);

        canvasGroup.alpha = 0f;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
