using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    [Header("Cheating")] // bold font 
    [SerializeField, Range(0f, 1f)] private float cheatChance = 0.25f; // npc cheating probability

    public CustomerState CurrentState { get; private set; } // current behaviour
    public bool IsCheater { get; private set; } // cheat flag
    public bool CanBeInteractedWith { get; private set; } = true; // npc interaction flag

    private NavMeshAgent agent; // navmesh 

    private void Awake() {
        agent = GetComponent<NavMeshAgent>(); // navmesh is a must for npc to make it walk
    }

    private void Start() {
        IsCheater = Random.value < cheatChance; // if random value is lower than prob, that npc is cheater
        CurrentState = CustomerState.Idle; // start state of npc (will change due its behaviour)
    }
}
