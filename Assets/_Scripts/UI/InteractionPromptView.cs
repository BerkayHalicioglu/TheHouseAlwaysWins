using UnityEngine;
using TMPro;

public class InteractionPromptView : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowPrompt(string message)
    {
        if (promptText != null)
            promptText.text = message;

        gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        gameObject.SetActive(false);
    }
}