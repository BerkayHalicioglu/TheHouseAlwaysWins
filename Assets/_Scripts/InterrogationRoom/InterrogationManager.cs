using System;
using UnityEngine;

public class InterrogationManager : MonoBehaviour
{
    public static event Action OnSuspectPunished;
    public static event Action OnSuspectReleased;

    private CustomerAI currentTarget;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetTarget(CustomerAI target)
    {
        currentTarget = target;
    }

    public void PunishSuspect()
    {
        CustomerAI target = currentTarget;
        OnSuspectPunished?.Invoke();
        EndInterrogation(() => target?.SendToBackRoom());
    }

    public void ReleaseSuspect()
    {
        CustomerAI target = currentTarget;
        OnSuspectReleased?.Invoke();
        EndInterrogation(() => target?.ReleaseFromInterrogation());
    }

    private void EndInterrogation(Action onReturnComplete)
    {
        currentTarget = null;
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.ReturnToCasino(onReturnComplete);
        else
            Debug.LogError("[InterrogationManager] SceneTransitionManager.Instance is null.");
    }
}
