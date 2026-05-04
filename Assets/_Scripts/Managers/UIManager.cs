using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // İbo bu metodları implement edecek
    public void ShowGameOverScreen() { }
    public void ShowDayEndReport() { }
    public void ShowCheaterCaughtFeedback() { }
    public void ShowWrongAccusationFeedback() { }
    public void UpdateHUD(float bankroll, float dayProgress) { }
}