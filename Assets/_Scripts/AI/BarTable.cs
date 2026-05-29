using UnityEngine;

public class BarTable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform[] waitPoints = new Transform[4];
    private CustomerAI[] occupiedPoints;

    [Header("Player Interaction")]
    [SerializeField] private float staminaRefillAmount = 50f;
    [SerializeField] private float interactionCooldown = 10f;

    private float cooldownTimer;

    public string InteractionPrompt => cooldownTimer <= 0f
        ? "Press E to Drink"
        : $"Wait {Mathf.CeilToInt(cooldownTimer)}s...";

    private void Awake()
    {
        occupiedPoints = new CustomerAI[waitPoints.Length];
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void Interact()
    {
        if (cooldownTimer > 0f) return;
        if (StaminaManager.Instance == null) return;

        StaminaManager.Instance.RefillStamina(staminaRefillAmount);
        cooldownTimer = interactionCooldown;
    }

    public bool ReservePoint(CustomerAI customer, out Transform point)
    {
        for (int i = 0; i < waitPoints.Length; i++)
        {
            if (waitPoints[i] == null)
            {
                continue;
            }

            if (occupiedPoints[i] == null)
            {
                occupiedPoints[i] = customer;
                point = waitPoints[i];
                return true;
            }
        }

        point = null;
        return false;
    }

    public void ReleasePoint(CustomerAI customer)
    {
        for (int i = 0; i < occupiedPoints.Length; i++)
        {
            if (occupiedPoints[i] == customer)
            {
                occupiedPoints[i] = null;
                return;
            }
        }
    }
}
