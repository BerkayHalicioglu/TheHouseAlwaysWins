using UnityEngine;

public class SuspiciousTarget : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject suspicionIndicator;

    public void Interact()
    {
        if (suspicionIndicator == null)
        {
            Debug.Log("No suspicion indicator assigned.");
            return;
        }

        if (suspicionIndicator.activeSelf)
        {
            Debug.Log("Correct suspect selected!");
        }
        else
        {
            Debug.Log("Wrong suspect selected!");
        }
    }
}