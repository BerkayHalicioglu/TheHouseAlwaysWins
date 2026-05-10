using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    [Header("Cheating")] // bold font 
    [SerializeField, Range(0f, 1f)] private float cheatChance = 0.25f; // npc cheating probability

    [Header("Movement")]
    [SerializeField] private float destinationReachedDistance = 0.5f;

    public CustomerState CurrentState { get; private set; } // current behaviour
    public bool IsCheater { get; private set; } // cheat flag
    public bool CanBeInteractedWith { get; private set; } = true; // npc interaction flag

    private NavMeshAgent agent; // navmesh 
    private BlackjackTable currentTable;
    private Transform currentSeat;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>(); // navmesh is a must for npc to make it walk
    }

    private void Start() {
        IsCheater = Random.value < cheatChance; // if random value is lower than prob, that npc is cheater
        CurrentState = CustomerState.Idle; // start state of npc (will change due its behaviour)

        if (!TryPlaceOnNavMesh())
        {
            return;
        }

        GoToBlackjackTable();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case CustomerState.WalkingToTable:
            UpdateWalkingToTable();
            break;

            case CustomerState.Leaving:
            UpdateLeaving();
            break;
        }
    }

    private void GoToBlackjackTable() // finds an empty blackjack seat
    {
        if (CustomerDestinationManager.Instance.TryGetBlackjackSeat(this, out currentTable, out currentSeat)) // is there any empty seat?
        {
            CurrentState = CustomerState.WalkingToTable; //if yes walks to there
            agent.SetDestination(currentSeat.position);
        }
        else
        {
            LeaveCasino(); // if no leaves casino
        }
    }

    private void UpdateWalkingToTable() // is npc reached to point?
    {
        if (HasReachedDestination())
        {
            CurrentState = CustomerState.PlayingBlackjack; // if yes npc stops and stars playing bj
            agent.ResetPath();
        }
    }

    private void LeaveCasino() // leave casino
    {
        if (currentTable != null)
        {
            currentTable.ReleaseSeat(this); // occupied seat reserved for another NPC
            currentTable = null;
            currentSeat = null;
        }

        CurrentState = CustomerState.Leaving; // npc starts walking to exit

        if (CustomerDestinationManager.Instance.ExitPoint != null)
        {
            agent.SetDestination(CustomerDestinationManager.Instance.ExitPoint.position);
        }
    }

    private void UpdateLeaving() // checks that is npc reached exit
      {
          if (HasReachedDestination()) // if yes npc deleted from scene
          {
              CurrentState = CustomerState.Gone;
              Destroy(gameObject);
          }
      }

      private bool HasReachedDestination() // if nacmesh still calculating distance the npc not yet left
      {
          if (agent.pathPending)
          {
              return false;
          }

          return agent.remainingDistance <= destinationReachedDistance; // returns true if npc gets closer to the exit
      }

      private bool TryPlaceOnNavMesh()
      {
          if (agent.isOnNavMesh)
          {
              return true;
          }

          if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
          {
              agent.Warp(hit.position);
              return true;
          }

          Debug.LogWarning($"{name} could not be placed on NavMesh.");
          return false;
      }
  
}
