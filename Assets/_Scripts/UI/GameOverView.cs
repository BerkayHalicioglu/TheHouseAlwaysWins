using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverView : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text daysSurvivedText;
    [SerializeField] private TMP_Text finalBankrollText;

    [Header("Buttons")]
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void Show(int daysSurvived, float finalBankroll)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (titleText != null)
            titleText.text = "GAME OVER";

        if (daysSurvivedText != null)
            daysSurvivedText.text = $"You survived {daysSurvived} day(s)";

        if (finalBankrollText != null)
            finalBankrollText.text = $"Final Bankroll:  ${finalBankroll:N0}";

        gameObject.SetActive(true);
    }

    private void OnTryAgainClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void OnQuitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}