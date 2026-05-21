using UnityEngine;
using UnityEngine.InputSystem;

public class QTEManager : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float qteTimeLimit = 3f;

    private Key expectedKey;
    private bool qteActive;
    private float timer;
    private PlayerController activePlayerController;



    public void StartQTE(PlayerController playerController)
    {
        activePlayerController = playerController;
        Debug.Log($"[QTE DEBUG] Received PlayerController: {(activePlayerController != null ? activePlayerController.gameObject.name : "NULL")}");

        expectedKey = GetRandomKey();
        timer = qteTimeLimit;
        qteActive = true;

        SetPlayerMovement(false);

        Debug.Log($"[QTE] Started. Press: {expectedKey}");
        Debug.Log("[UI HOOK] Show QTE prompt on screen");
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

        Debug.Log("[QTE] Success");
        Debug.Log("[UI HOOK] Show QTE success feedback");
    }

    private void FailQTE(string reason)
    {
        qteActive = false;
        SetPlayerMovement(true);

        Debug.Log($"[QTE] Failed: {reason}");
        Debug.Log("[UI HOOK] Show QTE failed feedback");
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (activePlayerController != null)
        {
            activePlayerController.SetMovementEnabled(enabled);
        }
        else
        {
            Debug.LogWarning("[QTE] Active PlayerController reference is missing.");
        }
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