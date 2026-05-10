// MARK: HOW MANY NPC CAN SIT ON A TABLE

using UnityEngine;

public class BlackjackTable : MonoBehaviour
{
    [SerializeField] private Transform[] seatPoints = new Transform[5]
    private customerAI[] occupiedSeats;
    
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
                occupiedSeats[i] == customer;
                seatPoints = seatPoints[i];
                return true;
            }
        }

        seatPoints = null;
        return false;
    }
}
