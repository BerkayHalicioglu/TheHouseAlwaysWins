using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    CasinoFloor,    // Normal oyun
    BackRoom,       // Arka oda sorgusu
    DayEndReport,   // Gün sonu raporu
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState CurrentState { get; private set; }

    // Events — diğer developerlar bunlara abone olacak
    public static event System.Action<GameState> OnGameStateChanged;
    public static event System.Action OnDayStarted;
    public static event System.Action OnDayEnded;
    public static event System.Action OnGameOver;

    [Header("Day Settings")]
    [SerializeField] private float dayDurationSeconds = 180f; // 3 dakika = 1 oyun günü
    private float dayTimer;
    public int CurrentDay { get; private set; } = 1;

    private bool dayEndPending = false;
    public bool IsInInterrogation { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Sahneler arası silinmez
    }
// oyun başlar başlamaz gün1 den alıyor (ibo)
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "CasinoFloor")
        {
            CurrentDay = 1;
            StartDay();
        }
        else
        {
            ChangeState(GameState.MainMenu);
        }
    }

//    private void Start()
//  {
//    ChangeState(GameState.MainMenu);
//}

    private void Update()
    {
        if (CurrentState == GameState.CasinoFloor)
        {
            dayTimer -= Time.deltaTime;
            if (dayTimer <= 0f)
            {
                EndDay();
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] State changed to: {newState}");
    }

    public void StartGame()
    {
        SaveManager.DeleteSave();
        CurrentDay = 1;
        IsInInterrogation = false;
        EconomyManager.Instance.Initialize(EconomyManager.Instance.StartingBankroll);
        StartDay();
        SceneManager.LoadScene("CasinoFloor");
    }

    public void LoadGame()
    {
        SaveData data = SaveManager.Load();
        if (data == null) { StartGame(); return; }

        CurrentDay = data.currentDay;
        EconomyManager.Instance.Initialize(data.currentBankroll);
        StartDay();
        SceneManager.LoadScene("CasinoFloor");
    }

    public void StartDay()
    {
        dayEndPending = false;
        dayTimer = dayDurationSeconds;
        ChangeState(GameState.CasinoFloor);
        OnDayStarted?.Invoke();
        Debug.Log($"[GameManager] Day {CurrentDay} started.");
    }

    public void EndDay()
    {
        if (IsInInterrogation)
        {
            dayEndPending = true;
            Debug.Log("[GameManager] Day ended during interrogation — waiting for player to return.");
            return;
        }

        CurrentDay++;
        SaveManager.Save(new SaveData
        {
            currentDay      = CurrentDay,
            currentBankroll = EconomyManager.Instance != null ? EconomyManager.Instance.CurrentBankroll : 0f
        });
        ChangeState(GameState.DayEndReport);
        OnDayEnded?.Invoke();
    }

    public void EnterBackRoom()
    {
        IsInInterrogation = true;
    }

    public void ExitBackRoom()
    {
        IsInInterrogation = false;

        if (dayEndPending)
        {
            dayEndPending = false;
            CurrentDay++;
            SaveManager.Save(new SaveData
            {
                currentDay      = CurrentDay,
                currentBankroll = EconomyManager.Instance != null ? EconomyManager.Instance.CurrentBankroll : 0f
            });
            ChangeState(GameState.DayEndReport);
            OnDayEnded?.Invoke();
            return;
        }
    }

    public void TriggerGameOver()
    {
        SaveManager.DeleteSave();
        ChangeState(GameState.GameOver);
        OnGameOver?.Invoke();
    }

    public float GetDayProgress() // İbo bunu HUD için kullanacak
    {
        return 1f - (dayTimer / dayDurationSeconds);
    }
}