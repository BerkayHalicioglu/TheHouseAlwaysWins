using UnityEngine;

public class BarTable : MonoBehaviour
{
    [SerializeField] private Transform[] waitPoints = new Transform[4];
    private CustomerAI[] occupiedPoints;

    private void Awake()
    {
        occupiedPoints = new CustomerAI[waitPoints.Length];
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
