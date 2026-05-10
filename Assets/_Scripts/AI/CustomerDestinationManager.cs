using UnityEngine;

public class CustomerDestinationManager : MonoBehaviour
{
    public static CustomerDestinationManager Instance { get; private set; }

    [SerializeField] private BlackjackTable[] blackjackTables; // list of blackjack tables
    [SerializeField] private Transform exitPoint; // exit point of casino

    public Transform ExitPoint => exitPoint;

    public int BlackjackTableCount => blackjackTables == null ? 0 : blackjackTables.Length;

    private void Awake()
    {
        Instance = this;
    }
    // walks around the tables to find empty seat. if found give out that point
    public bool TryGetBlackjackSeat(CustomerAI customer, out BlackjackTable table, out Transform seatPoint) {
        table = null;
        seatPoint = null;

        if (blackjackTables == null || blackjackTables.Length == 0)
        {
            return false;
        }
        int startIndex = Random.Range(0, blackjackTables.Length);

        for (int i = 0; i < blackjackTables.Length; i++)
        {
            int index = (startIndex + i) % blackjackTables.Length;
            BlackjackTable blackjackTable = blackjackTables[index];

            if (blackjackTable == null)
            {
                continue;
            }

            if (blackjackTable.ReserveSeat(customer, out seatPoint))
            {
                table = blackjackTable;
                return true;
            }
        }
        return false;
    }
}
