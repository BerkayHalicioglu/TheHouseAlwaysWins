using UnityEngine;

public class CustomerDestinationManager : MonoBehaviour
{
    public static CustomerDestinationManager Instance { get; private set; }

    [SerializeField] private BlackjackTable[] blackjackTables; // list of blackjack tables
    [SerializeField] private Transform exitPoint; // exit point of casino

    public Transform ExitPoint => exitPoint;

    private void Awake()
    {
        Instance = this;
    }
    // walks around the tables to find empty seat. if found give out that point
    public bool TryGetBlackjackSeat(CustomerAI customer, out BlackjackTable table, out Transform seatPoint) {
        foreach (BlackjackTable blackjackTable in blackjackTables) 
        {
            if (blackjackTable == null) // passes not determined seats
            {
                continue;
            }

            if (blackjackTable.ReserveSeat(customer, out seatPoint)) // is there any empty seat?
            {
                table = blackjackTable;
                return true;
            }
        }

        table = null;
        seatPoint = null;
        return false;
    }
}
