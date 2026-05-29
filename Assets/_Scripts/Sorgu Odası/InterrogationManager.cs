using System;
using UnityEngine;

public class InterrogationManager : MonoBehaviour
{
    public static event Action OnSuspectPunished;
    public static event Action OnSuspectReleased;
    public static event Action OnInterrogationComplete;

    private CustomerAI currentSuspect;

    public void SetSuspect(CustomerAI suspect)
    {
        currentSuspect = suspect;
    }

    public void PunishSuspect()
    {
        currentSuspect?.SendToBackRoom();
        OnSuspectPunished?.Invoke();
        EndInterrogation();
    }

    public void ReleaseSuspect()
    {
        currentSuspect?.ReleaseFromInterrogation();
        OnSuspectReleased?.Invoke();
        EndInterrogation();
    }

    private void EndInterrogation()
    {
        currentSuspect = null;
        OnInterrogationComplete?.Invoke();
    }
}
