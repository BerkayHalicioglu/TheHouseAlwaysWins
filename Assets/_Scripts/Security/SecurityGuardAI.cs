using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SecurityGuardAI : MonoBehaviour
{
    [Header("Devriye Ayarlarý")]
    [SerializeField] private Transform[] waypoints;

    [Header("Bekleme Ayarlarý")]
    [Tooltip("Masada en az kaç saniye beklesin?")]
    public float minWaitTime = 5f;
    [Tooltip("Masada en çok kaç saniye beklesin?")]
    public float maxWaitTime = 15f;

    public Transform[] Waypoints => waypoints;
    public int CurrentWaypointIndex { get; set; }

    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }

    private IGuardStrategy _currentStrategy;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            CurrentWaypointIndex = Random.Range(0, waypoints.Length);
        }

        ChangeStrategy(new PatrolStrategy());
    }

    private void Update()
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.UpdateStrategy(this);
        }
    }

    public void ChangeStrategy(IGuardStrategy newStrategy)
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.ExitStrategy(this);
        }

        _currentStrategy = newStrategy;
        _currentStrategy.EnterStrategy(this);
    }

    public float GetRandomWaitTime()
    {
        return Random.Range(minWaitTime, maxWaitTime);
    }
}