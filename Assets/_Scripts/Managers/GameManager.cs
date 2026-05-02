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

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

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
        CurrentDay = 1;
        StartDay();
        SceneManager.LoadScene("CasinoFloor");
    }

    public void StartDay()
    {
        dayTimer = dayDurationSeconds;
        ChangeState(GameState.CasinoFloor);
        OnDayStarted?.Invoke();
        Debug.Log($"[GameManager] Day {CurrentDay} started.");
    }

    public void EndDay()
    {
        CurrentDay++;
        ChangeState(GameState.DayEndReport);
        OnDayEnded?.Invoke();
    }

    public void EnterBackRoom()
    {
        ChangeState(GameState.BackRoom);
    }

    public void ExitBackRoom()
    {
        ChangeState(GameState.CasinoFloor);
    }

    public void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);
        OnGameOver?.Invoke();
    }

    public float GetDayProgress() // İbo bunu HUD için kullanacak
    {
        return 1f - (dayTimer / dayDurationSeconds);
    }
}