using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterrogationView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button guiltyButton;
    [SerializeField] private Button innocentButton;
    [SerializeField] private TMP_Text headerText;

    private CustomerAI currentTarget;

    private void Awake()
    {
        gameObject.SetActive(false);
        guiltyButton?.onClick.AddListener(OnGuiltyPressed);
        innocentButton?.onClick.AddListener(OnInnocentPressed);
        QTEManager.OnInterrogationQTESuccess += Show;
    }

    private void OnDestroy()
    {
        QTEManager.OnInterrogationQTESuccess -= Show;
    }

    private void Show(CustomerAI target)
    {
        currentTarget = target;

        if (headerText != null)
            headerText.text = "SUSPECT DETAINED\nGuilty or Innocent?";

        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Hide()
    {
        currentTarget = null;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnGuiltyPressed()
    {
        currentTarget?.SendToBackRoom();
        Hide();
    }

    private void OnInnocentPressed()
    {
        currentTarget?.ReleaseFromInterrogation();
        Hide();
    }
}
