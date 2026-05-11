using UnityEngine;

public class CustomerDestinationManager : MonoBehaviour
{
    public static CustomerDestinationManager Instance { get; private set; }

    [SerializeField] private BlackjackTable[] blackjackTables; // list of blackjack tables
    [SerializeField] private SlotMachine[] slotMachines; // list of slot machines
    [SerializeField] private RouletteTable[] rouletteTables; // list of roulette tables
    [SerializeField] private BarTable[] barTables; // list of bar tables
    [SerializeField] private Transform exitPoint; // exit point of casino

    public Transform ExitPoint => exitPoint;

    public int BlackjackTableCount => blackjackTables == null ? 0 : blackjackTables.Length;
    public int SlotMachineCount => slotMachines == null ? 0 : slotMachines.Length;
    public int RouletteTableCount => rouletteTables == null ? 0 : rouletteTables.Length;
    public int BarTableCount => barTables == null ? 0 : barTables.Length;

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
    // same logic with trygetblackjackseat but for slot machines
    public bool TryGetSlotPoint(CustomerAI customer, out SlotMachine machine, out Transform slotPoint)
    {
        machine = null;
        slotPoint = null;

        if (slotMachines == null || slotMachines.Length == 0)
        {
            return false;
        }
        int startIndex = Random.Range(0, slotMachines.Length);

        for (int i = 0; i < slotMachines.Length; i++)
        {
            int index = (startIndex + i) % slotMachines.Length;
            SlotMachine slotMachine = slotMachines[index];

            if (slotMachine == null)
            {
                continue;
            }

            if (slotMachine.ReserveSlot(customer, out slotPoint))
            {
                machine = slotMachine;
                return true;
            }
        }
        return false;
    }
    // also same with other 2 method but for roulette
    public bool TryGetRouletteSeat(CustomerAI customer, out RouletteTable table, out Transform seatPoint)
    {
        table = null;
        seatPoint = null;

        if (rouletteTables == null || rouletteTables.Length == 0)
        {
            return false;
        }

        int startIndex = Random.Range(0, rouletteTables.Length);

        for (int i = 0; i < rouletteTables.Length; i++)
        {
            int index = (startIndex + i) % rouletteTables.Length;
            RouletteTable rouletteTable = rouletteTables[index];

            if (rouletteTable == null)
            {
                continue;
            }

            if (rouletteTable.ReserveSeat(customer, out seatPoint))
            {
                table = rouletteTable;
                return true;
            }
        }

        return false;
    }

    // also same with other 2 method but for bar table
    public bool TryGetBarPoint(CustomerAI customer, out BarTable table, out Transform seatPoint)
    {
        table = null;
        seatPoint = null;

        if (barTables == null || barTables.Length == 0)
        {
            return false;
        }

        int startIndex = Random.Range(0, barTables.Length);

        for (int i = 0; i < barTables.Length; i++)
        {
            int index = (startIndex + i) % barTables.Length;
            BarTable barTable = barTables[index];

            if (barTable == null)
            {
                continue;
            }

            if (barTable.ReservePoint(customer, out seatPoint))
            {
                table = barTable;
                return true;
            }
        }

        return false;
    }
}
