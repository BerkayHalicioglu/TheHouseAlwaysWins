using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject feedbackPanel;
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
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    public void ShowCheaterCaught(float amount)
    {
        if (titleText != null)
        {
            titleText.text = "CHEATER CAUGHT!";
            titleText.color = Color.green;
        }

        if (amountText != null)
        {
            amountText.text = $"+${amount:N0}";
            amountText.color = Color.green;
        }

        PlaySound(successSound);
        Show();
    }

    public void ShowWrongAccusation(float amount)
    {
        if (titleText != null)
        {
            titleText.text = "WRONG ACCUSATION!";
            titleText.color = Color.red;
        }

        if (amountText != null)
        {
            amountText.text = $"-${amount:N0}";
            amountText.color = Color.red;
        }

        PlaySound(errorSound);
        Show();
    }

    public void ShowInnocentCompensation(float amount)
    {
        if (titleText != null)
        {
            titleText.text = "THEY WERE INNOCENT!";
            titleText.color = Color.red;
        }

        if (amountText != null)
        {
            amountText.text = $"-${amount:N0} Compensation";
            amountText.color = Color.red;
        }

        PlaySound(errorSound);
        Show();
    }

    public void ShowCorrectRelease(float amount)
    {
        if (titleText != null)
        {
            titleText.text = "CORRECT CALL!";
            titleText.color = Color.green;
        }

        if (amountText != null)
        {
            amountText.text = $"+${amount:N0}";
            amountText.color = Color.green;
        }

        PlaySound(successSound);
        Show();
    }

    public void ShowCheaterMissed(float amount)
    {
        Color orange = new Color(1f, 0.5f, 0f);

        if (titleText != null)
        {
            titleText.text = "CHEATER ESCAPED!";
            titleText.color = orange;
        }

        if (amountText != null)
        {
            amountText.text = $"-${amount:N0}";
            amountText.color = orange;
        }

        PlaySound(errorSound);
        Show();
    }

    private void Show()
    {
        StopAllCoroutines();

        if (feedbackPanel != null)
            feedbackPanel.SetActive(true);

        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}