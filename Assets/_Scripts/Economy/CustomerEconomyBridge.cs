using UnityEngine;

[RequireComponent(typeof(CustomerAI))]
public class CustomerEconomyBridge : MonoBehaviour
{
    [Header("NPC Bet Settings")]
    [SerializeField] private float minNpcBet = 50f;
    [SerializeField] private float maxNpcBet = 250f;
    [SerializeField, Range(0f, 1f)] private float npcWinChance = 0.4f;

    private CustomerAI customerAI;
    private CustomerState previousState;
    private bool gameResolved;

    private void Awake()
    {
        customerAI = GetComponent<CustomerAI>();
    }

    // Pool'dan geri alındığında (SetActive true) state sıfırlanır
    private void OnEnable()
    {
        previousState = CustomerState.Idle;
        gameResolved  = false;
    }

    private void Update()
    {
        CustomerState currentState = customerAI.CurrentState;

        if (!gameResolved &&
            previousState == CustomerState.PlayingActivity &&
            (currentState == CustomerState.Leaving || currentState == CustomerState.Gone))
        {
            ResolveNpcEconomy();
            gameResolved = true;
        }

        previousState = currentState;
    }

    private void ResolveNpcEconomy()
    {
        if (EconomyManager.Instance == null) return;

        float betAmount = Random.Range(minNpcBet, maxNpcBet);

        // Decorator pattern: hileci müşteri StandardResolver'ın üzerine ek kazanma katmanı ekler
        IEconomyResolver resolver = new StandardEconomyResolver(npcWinChance);
        if (customerAI.IsCheater)
            resolver = new CheaterEconomyDecorator(resolver);

        resolver.Resolve(betAmount);
    }
}
