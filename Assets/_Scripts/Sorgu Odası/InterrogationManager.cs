using System;
using UnityEngine;

public class InterrogationManager : MonoBehaviour
{
    public static event Action OnSuspectPunished;
    public static event Action OnSuspectReleased;

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PunishSuspect()
    {
        Debug.Log("SÝSTEM: Oyuncu 'DÖV' kararýný verdi!");

        OnSuspectPunished?.Invoke();

        EndInterrogation();
    }

    public void ReleaseSuspect()
    {
        Debug.Log("SÝSTEM: Oyuncu 'SERBEST BIRAK' kararýný verdi!");

        OnSuspectReleased?.Invoke();

        EndInterrogation();
    }

    private void EndInterrogation()
    {
        gameObject.SetActive(false);
    }
}