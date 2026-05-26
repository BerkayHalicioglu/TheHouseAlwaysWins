using UnityEngine;
using UnityEngine.InputSystem;

public class QTEManager : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float qteTimeLimit = 3f;
    [SerializeField] private PlayerController playerController;

    private Key expectedKey;
    private bool qteActive;
    private float timer;
    private CustomerAI currentTarget;

    public static event System.Action<UnityEngine.InputSystem.Key, float> OnQTEStarted;
    public static event System.Action OnQTESuccess;
    public static event System.Action OnQTEFailed;
    public static event System.Action<CustomerAI> OnInterrogationQTESuccess;

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
        if (playerController == null)
            Debug.LogWarning("[QTE] PlayerController is not assigned.");

        expectedKey = GetRandomKey();
        timer = qteTimeLimit;
        qteActive = true;

        SetPlayerMovement(false);
        OnQTEStarted?.Invoke(expectedKey, qteTimeLimit);
    }

    private void Update()
    {
        if (!qteActive)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            FailQTE("Time expired");
            return;
        }

        CheckInput();
    }

    private void CheckInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
            EvaluateInput(Key.I);

        if (Keyboard.current.jKey.wasPressedThisFrame)
            EvaluateInput(Key.J);

        if (Keyboard.current.kKey.wasPressedThisFrame)
            EvaluateInput(Key.K);

        if (Keyboard.current.lKey.wasPressedThisFrame)
            EvaluateInput(Key.L);
    }

    private void EvaluateInput(Key pressedKey)
    {
        if (pressedKey == expectedKey)
        {
            SuccessQTE();
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

    private Key GetRandomKey()
    {
        int randomIndex = Random.Range(0, 4);

        switch (randomIndex)
        {
            case 0: return Key.I;
            case 1: return Key.J;
            case 2: return Key.K;
            default: return Key.L;
        }
    }
}