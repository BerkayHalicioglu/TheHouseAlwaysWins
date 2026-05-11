using UnityEngine;

public class RouletteTable : MonoBehaviour
{
    [SerializeField] private Transform[] seatPoints = new Transform[5];
    private CustomerAI[] occupiedSeats;
    
    private void Awake() 
    {
        occupiedSeats = new CustomerAI[seatPoints.Length];
    }
    // npc checks empty seats
    public bool ReserveSeat(CustomerAI customer, out Transform seatpoint)
    {
        // search for table length
        for (int i = 0; i < seatPoints.Length; i++)
        {   // inspector seatpoint 
            if (seatPoints[i] == null)
            {
                continue;
            }
            // is seat empty?
            if (occupiedSeats[i] == null)
            {
                occupiedSeats[i] = customer;
                seatpoint = seatPoints[i];
                return true;
            }
        }

        seatpoint = null;
        return false;
    }
    // npc leaving the table
    public void ReleaseSeat(CustomerAI customer)
    {
        // search for occupied seats
        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            // is seat occupied by a NPC
            if (occupiedSeats[i] == customer)
            {   // if yes empty seat
                occupiedSeats[i] = null;
                return;
            }
        }
    }
}
