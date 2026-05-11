using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private CustomerAI customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int targetCustomerCount = 24;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float navMeshSearchDistance = 5f;

    private float spawnTimer;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.CasinoFloor)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer < spawnInterval)
        {
            return;
        }

        spawnTimer = 0f;

        if (CountCustomers() >= targetCustomerCount)
        {
            return;
        }

        SpawnCustomer();
    }

    private int CountCustomers()
    {
        return FindObjectsByType<CustomerAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null || spawnPoint == null)
        {
            return;
        }

        Vector3 spawnPosition = spawnPoint.position;

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, navMeshSearchDistance, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        Instantiate(customerPrefab, spawnPosition, spawnPoint.rotation);
    }
}
