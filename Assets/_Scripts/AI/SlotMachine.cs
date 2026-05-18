using UnityEngine;
using UnityEngine.AI;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private Transform usePoint;
    [SerializeField] private float standDistance = 1.1f;
    [SerializeField] private float navMeshSearchDistance = 0.8f;

    private CustomerAI occupiedCustomer;
    private Transform runtimeUsePoint;

    private void Awake()
    {
        FindUsePoint();
    }

    public bool ReserveSlot(CustomerAI customer, out Transform slotPoint)
    {
        if (occupiedCustomer != null)
        {
            slotPoint = null;
            return false;
        }

        FindUsePoint();
        occupiedCustomer = customer;
        slotPoint = GetBestUsePoint();
        return true;
    }

    public void ReleaseSlot(CustomerAI customer)
    {
        if (occupiedCustomer == customer)
        {
            occupiedCustomer = null;
        }
    }

    private void FindUsePoint()
    {
        if (usePoint == null)
        {
            usePoint = transform.Find("UsePoint");
        }
    }

    private Transform GetBestUsePoint()
    {
        if (runtimeUsePoint == null)
        {
            GameObject pointObject = new GameObject("RuntimeUsePoint");
            pointObject.transform.SetParent(transform);
            runtimeUsePoint = pointObject.transform;
        }

        Vector3 fallbackPosition = usePoint != null ? usePoint.position : transform.position;
        runtimeUsePoint.position = FindWalkableUsePosition(fallbackPosition);
        runtimeUsePoint.rotation = transform.rotation;
        return runtimeUsePoint;
    }

    private Vector3 FindWalkableUsePosition(Vector3 fallbackPosition)
    {
        Vector3[] directions =
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 candidatePosition = transform.position + directions[i] * standDistance;

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, navMeshSearchDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        if (NavMesh.SamplePosition(fallbackPosition, out NavMeshHit fallbackHit, navMeshSearchDistance, NavMesh.AllAreas))
        {
            return fallbackHit.position;
        }

        return fallbackPosition;
    }
}
