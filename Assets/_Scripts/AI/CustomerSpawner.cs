using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private CustomerAI customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int targetCustomerCount = 24;
    [SerializeField] private int minimumCustomerCount = 18;
    [SerializeField] private int maxSpawnPerTick = 3;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float navMeshSearchDistance = 5f;

    private float spawnTimer;

    private void OnEnable()
    {
        DifficultyManager.OnDifficultyUpdated += ApplyDifficulty;
    }

    private void OnDisable()
    {
        DifficultyManager.OnDifficultyUpdated -= ApplyDifficulty;
    }

    private void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null) return;
        spawnInterval = DifficultyManager.Instance.SpawnInterval;
        targetCustomerCount = DifficultyManager.Instance.TargetCustomerCount;
    }

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

        int currentCustomerCount = CountCustomers();

        if (currentCustomerCount >= targetCustomerCount)
        {
            return;
        }

        int missingCustomerCount = targetCustomerCount - currentCustomerCount;

        if (currentCustomerCount < minimumCustomerCount)
        {
            int spawnCount = Mathf.Min(missingCustomerCount, maxSpawnPerTick);

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnCustomer();
            }

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

        if (CustomerPool.Instance != null)
            CustomerPool.Instance.Get(spawnPosition, spawnPoint.rotation);
        else
            Instantiate(customerPrefab, spawnPosition, spawnPoint.rotation);
    }
}
