using System.Collections.Generic;
using UnityEngine;

// Object Pool: her spawn'da Instantiate/Destroy yerine nesneleri havuzdan alıp geri bırakır.
// CasinoFloor sahnesinde yaşar, sahne yenilendiğinde havuz da yenilenir.
public class CustomerPool : MonoBehaviour
{
    public static CustomerPool Instance { get; private set; }

    [SerializeField] private CustomerAI customerPrefab;
    [SerializeField] private int initialPoolSize = 15;

    private readonly Queue<CustomerAI> pool = new Queue<CustomerAI>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Prewarm();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CustomerAI customer = CreateNew();
            customer.gameObject.SetActive(false);
            pool.Enqueue(customer);
        }
    }

    public CustomerAI Get(Vector3 position, Quaternion rotation)
    {
        CustomerAI customer = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        customer.gameObject.SetActive(true);
        customer.Initialize(position, rotation);
        return customer;
    }

    public void Return(CustomerAI customer)
    {
        customer.gameObject.SetActive(false);
        pool.Enqueue(customer);
    }

    private CustomerAI CreateNew()
    {
        return Instantiate(customerPrefab);
    }
}
