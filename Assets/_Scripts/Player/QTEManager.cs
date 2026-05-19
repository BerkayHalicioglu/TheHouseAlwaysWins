using UnityEngine;
using UnityEngine.InputSystem;

public class QTEManager : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float qteTimeLimit = 3f;

    private Key expectedKey;
    private bool qteActive;
    private float timer;

    public void StartQTE()
    {
        expectedKey = GetRandomKey();
        timer = qteTimeLimit;
        qteActive = true;

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

        if (Keyboard.current.wKey.wasPressedThisFrame)
            EvaluateInput(Key.W);

        if (Keyboard.current.aKey.wasPressedThisFrame)
            EvaluateInput(Key.A);

        if (Keyboard.current.sKey.wasPressedThisFrame)
            EvaluateInput(Key.S);

        if (Keyboard.current.dKey.wasPressedThisFrame)
            EvaluateInput(Key.D);
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
        Debug.Log("[QTE] Success");
        Debug.Log("[UI HOOK] Show QTE success feedback");
    }

    private void FailQTE(string reason)
    {
        qteActive = false;
        Debug.Log($"[QTE] Failed: {reason}");
        Debug.Log("[UI HOOK] Show QTE failed feedback");
    }

    private Key GetRandomKey()
    {
        int randomIndex = Random.Range(0, 4);

        switch (randomIndex)
        {
            case 0: return Key.W;
            case 1: return Key.A;
            case 2: return Key.S;
            default: return Key.D;
        }
    }
}