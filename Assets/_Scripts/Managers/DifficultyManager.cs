using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Scaling Range (Day 1 → Day Cap)")]
    [SerializeField] private int dayCap = 10;

    [Header("Cheat Chance")]
    [SerializeField] private float cheatChanceDay1 = 0.25f;
    [SerializeField] private float cheatChanceDayCap = 0.45f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnIntervalDay1 = 3f;
    [SerializeField] private float spawnIntervalDayCap = 1.5f;
    [SerializeField] private int customerCountDay1 = 25;
    [SerializeField] private int customerCountDayCap = 35;

    [Header("False Suspicion Chance")]
    [SerializeField] private float falseSuspicionChanceDay1 = 0.1f;
    [SerializeField] private float falseSuspicionChanceDayCap = 0.25f;

    [Header("Daily Expense Multiplier")]
    [SerializeField] private float expenseMultiplierDay1 = 1f;
    [SerializeField] private float expenseMultiplierDayCap = 2f;

    [Header("QTE Settings")]
    [SerializeField] private int qteSequenceLengthDay1 = 4;
    [SerializeField] private int qteSequenceLengthDayCap = 8;
    [SerializeField] private float qteTimePerKeyDay1 = 3f;
    [SerializeField] private float qteTimePerKeyDayCap = 1.8f;

    public float CheatChance { get; private set; }
    public float FalseSuspicionChance { get; private set; }
    public float SpawnInterval { get; private set; }
    public int TargetCustomerCount { get; private set; }
    public float ExpenseMultiplier { get; private set; }
    public int QTESequenceLength { get; private set; }
    public float QTETimePerKey { get; private set; }

    public static event System.Action OnDifficultyUpdated;

    private void Awake() // burayı fixledim artık hileciler görünecek 
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        UpdateDifficulty();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameManager.OnDayStarted += UpdateDifficulty;
    }

    private void OnDisable()
    {
        GameManager.OnDayStarted -= UpdateDifficulty;
    }

    private void Start()
    {
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        int day = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;
        float t = Mathf.Clamp01((float)(day - 1) / Mathf.Max(1, dayCap - 1));

        CheatChance          = Mathf.Lerp(cheatChanceDay1,          cheatChanceDayCap,          t);
        FalseSuspicionChance = Mathf.Lerp(falseSuspicionChanceDay1, falseSuspicionChanceDayCap, t);
        SpawnInterval        = Mathf.Lerp(spawnIntervalDay1,        spawnIntervalDayCap,        t);
        TargetCustomerCount  = Mathf.RoundToInt(Mathf.Lerp(customerCountDay1, customerCountDayCap, t));
        ExpenseMultiplier    = Mathf.Lerp(expenseMultiplierDay1,    expenseMultiplierDayCap,    t);
        QTESequenceLength    = Mathf.RoundToInt(Mathf.Lerp(qteSequenceLengthDay1, qteSequenceLengthDayCap, t));
        QTETimePerKey        = Mathf.Lerp(qteTimePerKeyDay1,        qteTimePerKeyDayCap,        t);

        OnDifficultyUpdated?.Invoke();

        Debug.Log($"[Difficulty] Day {day} — Cheat:{CheatChance:P0} FalseSuspicion:{FalseSuspicionChance:P0} Spawn:{SpawnInterval:F1}s Customers:{TargetCustomerCount} ExpenseX:{ExpenseMultiplier:F2}");
    }
}
