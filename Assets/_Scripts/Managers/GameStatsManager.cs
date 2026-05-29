using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    public int CheatersCaught  { get; private set; }
    public int InnocentsSent   { get; private set; }
    public int CheatersMissed  { get; private set; }
    public int CorrectReleases { get; private set; }
    public float DayStartBankroll { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        CustomerAI.OnCheaterCaught            += HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom   += HandleInnocentSent;
        CustomerAI.OnCheaterMissed            += HandleCheaterMissed;
        CustomerAI.OnCorrectRelease           += HandleCorrectRelease;
        GameManager.OnDayStarted              += HandleDayStarted;
    }

    private void OnDisable()
    {
        CustomerAI.OnCheaterCaught            -= HandleCheaterCaught;
        CustomerAI.OnInnocentSentToBackRoom   -= HandleInnocentSent;
        CustomerAI.OnCheaterMissed            -= HandleCheaterMissed;
        CustomerAI.OnCorrectRelease           -= HandleCorrectRelease;
        GameManager.OnDayStarted              -= HandleDayStarted;
    }

    private void HandleDayStarted()
    {
        CheatersCaught  = 0;
        InnocentsSent   = 0;
        CheatersMissed  = 0;
        CorrectReleases = 0;
        DayStartBankroll = EconomyManager.Instance != null
            ? EconomyManager.Instance.CurrentBankroll
            : 0f;
    }

    private void HandleCheaterCaught(float _)  => CheatersCaught++;
    private void HandleInnocentSent(float _)    => InnocentsSent++;
    private void HandleCheaterMissed(float _)   => CheatersMissed++;
    private void HandleCorrectRelease(float _)  => CorrectReleases++;

    public DayStats BuildStats()
    {
        return new DayStats
        {
            cheatersCaught  = CheatersCaught,
            innocentsSent   = InnocentsSent,
            cheatersMissed  = CheatersMissed,
            correctReleases = CorrectReleases,
            bankrollStart   = DayStartBankroll,
            bankrollEnd     = EconomyManager.Instance != null
                ? EconomyManager.Instance.CurrentBankroll
                : 0f
        };
    }
}
