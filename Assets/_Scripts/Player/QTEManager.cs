using UnityEngine;
using UnityEngine.InputSystem;

public class QTEManager : MonoBehaviour
{
    [Header("QTE Fallback Settings")]
    [SerializeField] private float fallbackTimePerKey = 3f;
    [SerializeField] private PlayerController playerController;

    private static readonly Key[] KeyPool =
    {
        // Üst sıra
        Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y, Key.U, Key.O, Key.P,
        // Orta sıra
        Key.A, Key.S, Key.D, Key.F, Key.G, Key.H, Key.J, Key.K, Key.L,
        // Alt sıra
        Key.Z, Key.X, Key.C, Key.V, Key.B, Key.N, Key.M,
    };

    private Key[] sequence;
    private int   currentStep;
    private bool  qteActive;
    private float timer;
    private CustomerAI currentTarget;

    public static event System.Action<Key[], float> OnQTEStarted;
    public static event System.Action<int>          OnQTEStepAdvanced;
    public static event System.Action               OnQTESuccess;
    public static event System.Action               OnQTEFailed;
    public static event System.Action<CustomerAI>   OnInterrogationQTESuccess;

    private void OnEnable()
    {
        CustomerAI.OnInterrogationStarted += HandleInterrogationStarted;
    }

    private void OnDisable()
    {
        CustomerAI.OnInterrogationStarted -= HandleInterrogationStarted;
    }

    private void HandleInterrogationStarted(CustomerAI target)
    {
        currentTarget = target;
        StartQTE();
    }

    private void StartQTE()
    {
        int   length     = DifficultyManager.Instance != null ? DifficultyManager.Instance.QTESequenceLength : 1;
        float timePerKey = DifficultyManager.Instance != null ? DifficultyManager.Instance.QTETimePerKey      : fallbackTimePerKey;

        sequence    = BuildSequence(length);
        currentStep = 0;
        timer       = timePerKey * length;
        qteActive   = true;

        SetPlayerMovement(false);
        OnQTEStarted?.Invoke(sequence, timer);
    }

    private void Update()
    {
        if (!qteActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) { FailQTE("Time expired"); return; }

        CheckInput();
    }

    private void CheckInput()
    {
        if (Keyboard.current == null) return;

        foreach (Key key in KeyPool)
        {
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                EvaluateInput(key);
                return;
            }
        }
    }

    private void EvaluateInput(Key pressed)
    {
        if (pressed == sequence[currentStep])
        {
            currentStep++;
            if (currentStep >= sequence.Length)
                SuccessQTE();
            else
                OnQTEStepAdvanced?.Invoke(currentStep);
        }
        else
        {
            FailQTE("Wrong key pressed");
        }
    }

    private void SuccessQTE()
    {
        qteActive = false;
        SetPlayerMovement(true);
        OnQTESuccess?.Invoke();
        OnInterrogationQTESuccess?.Invoke(currentTarget);
        currentTarget = null;
    }

    private void FailQTE(string reason)
    {
        qteActive = false;
        SetPlayerMovement(true);
        OnQTEFailed?.Invoke();
        currentTarget?.MissCheater();
        currentTarget = null;
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerController != null)
            playerController.SetMovementEnabled(enabled);
        else
            Debug.LogWarning("[QTE] PlayerController reference is missing.");
    }

    private static Key[] BuildSequence(int length)
    {
        Key[] seq = new Key[length];
        for (int i = 0; i < length; i++)
            seq[i] = KeyPool[Random.Range(0, KeyPool.Length)];
        return seq;
    }
}
