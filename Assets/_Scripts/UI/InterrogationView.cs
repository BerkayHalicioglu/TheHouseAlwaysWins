using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterrogationView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button guiltyButton;
    [SerializeField] private Button innocentButton;
    [SerializeField] private TMP_Text headerText;

    [Header("References")]
    [SerializeField] private InterrogationManager interrogationManager;

    private void Awake()
    {
        if (interrogationManager == null)
            interrogationManager = GetComponentInParent<InterrogationManager>(true)
                                ?? Object.FindFirstObjectByType<InterrogationManager>(FindObjectsInactive.Include);

        guiltyButton?.onClick.AddListener(OnGuiltyPressed);
        innocentButton?.onClick.AddListener(OnInnocentPressed);
    }

    public void Show(CustomerAI target)
    {
        if (headerText != null)
            headerText.text = "SUSPECT DETAINED\nGuilty or Innocent?";
    }

    private void OnGuiltyPressed()
    {
        interrogationManager?.PunishSuspect();
    }

    private void OnInnocentPressed()
    {
        interrogationManager?.ReleaseSuspect();
    }
}
