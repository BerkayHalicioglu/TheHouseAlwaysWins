using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class CustomerAI : MonoBehaviour, IInteractable
{
    private const int ActivityCount = 4;

    [Header("Cheating")] // bold font 
    [SerializeField, Range(0f, 1f)] private float cheatChance = 0.25f; // npc cheating probability
    [SerializeField] private float minPlayBeforeCheat = 5f; // after 5sc that player cheats
    [SerializeField] private float minPlayAfterCheat = 15f; // last for 15 seconds
    [SerializeField] private float minActivityDuration = 20f;
    [SerializeField] private float maxActivityDuration = 45f;


    [Header("Movement")]
    [SerializeField] private float destinationReachedDistance = 0.5f;
    [SerializeField] private float destinationSearchDistance = 3f;

    public CustomerState CurrentState { get; private set; } // current behaviour
    public bool IsCheater { get; private set; } // cheat flag
    public bool CanBeInteractedWith { get; private set; } = true; // npc interaction flag

    private NavMeshAgent agent; // navmesh 
    private BlackjackTable currentTable;
    private Transform currentSeat;
    private CustomerActivity currentActivity;

    private SlotMachine currentSlotMachine; // slot machine
    private RouletteTable currentRouletteTable; // roulette table
    private BarTable currentBarTable; // bar table

    private float playTimer; // counts play time
    private float cheatTimer; // counts cheat time
    private float activityTimer;

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

        ChooseActivity();
        GoToSelectedActivity();
    }

    private void Update()
    {
        if (!IsCasinoRunning())
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            return;
        }

        if (agent.isOnNavMesh && agent.isStopped && (CurrentState == CustomerState.WalkingToTable || CurrentState == CustomerState.Leaving))
        {
            agent.isStopped = false;
        }

        switch (CurrentState)
        {
            case CustomerState.WalkingToTable:
            UpdateWalkingToTable();
            break;

            case CustomerState.Leaving:
            UpdateLeaving();
            break;

            case CustomerState.PlayingBlackjack: // timer starts when NPC playing
            UpdatePlayingBlackjack();
            break;
        }
    }

    private void ChooseActivity() // npc activity probabilty
    {
        int activityRoll = Random.Range(0, 100);

        if (activityRoll < 20)
        {
            currentActivity = CustomerActivity.Blackjack;
        }
        else if (activityRoll < 75)
        {
            currentActivity = CustomerActivity.Slot;
        }
        else if (activityRoll < 90)
        {
            currentActivity = CustomerActivity.Roulette;
        }
        else
        {
            currentActivity = CustomerActivity.Bar;
        }
    }

    private void GoToSelectedActivity() // npc go mechanics
    {
        int startActivityIndex = (int)currentActivity;

        for (int i = 0; i < ActivityCount; i++)
        {
            currentActivity = (CustomerActivity)((startActivityIndex + i) % ActivityCount);

            if (TryGoToCurrentActivity())
            {
                return;
            }
        }

        LeaveCasino();
    }

    private bool TryGoToCurrentActivity()
    {
        switch (currentActivity)
        {
            case CustomerActivity.Blackjack:
            return TryGoToBlackjackTable();

            case CustomerActivity.Slot:
            return TryGoToSlotMachine();

            case CustomerActivity.Roulette:
            return TryGoToRouletteTable();

            case CustomerActivity.Bar:
            return TryGoToBarPoint();

            case CustomerActivity.Exit:
            LeaveCasino();
            return true;
        }

        return false;
    }

    private bool TryGoToBlackjackTable() // finds an empty blackjack seat
    {
        int maxAttempts = CustomerDestinationManager.Instance.BlackjackTableCount;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!CustomerDestinationManager.Instance.TryGetBlackjackSeat(this, out currentTable, out currentSeat)) // is there any empty seat check
            {
                break;
            }

            if (SetDestinationOnNavMesh(currentSeat.position))
            {
                CurrentState = CustomerState.WalkingToTable; // if yes walks to table
                return true;
            }

            currentTable.ReleaseSeat(this);
            currentTable = null;
            currentSeat = null;
        }

        return false;
    }

    private bool TryGoToSlotMachine() // finds an empty slot machine
    {
        int maxAttempts = CustomerDestinationManager.Instance.SlotMachineCount;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!CustomerDestinationManager.Instance.TryGetSlotPoint(this, out currentSlotMachine, out currentSeat)) // is there any empty seat check
            {
                break;
            }

            if (SetDestinationOnNavMesh(currentSeat.position))
            {
                CurrentState = CustomerState.WalkingToTable; // if yes walks to table
                return true;
            }

            currentSlotMachine.ReleaseSlot(this);
            currentSlotMachine = null;
            currentSeat = null;
        }

        return false;
    }

    private bool TryGoToRouletteTable() // finds an empty roulette seat
    {
        int maxAttempts = CustomerDestinationManager.Instance.RouletteTableCount;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!CustomerDestinationManager.Instance.TryGetRouletteSeat(this, out currentRouletteTable, out currentSeat)) // is there any empty seat check
            {
                break;
            }

            if (SetDestinationOnNavMesh(currentSeat.position))
            {
                CurrentState = CustomerState.WalkingToTable; // if yes walks to table
                return true;
            }

            currentRouletteTable.ReleaseSeat(this);
            currentRouletteTable = null;
            currentSeat = null;
        }

        return false;
    }

    private bool TryGoToBarPoint() // finds an empty blackjack seat
    {
        int maxAttempts = CustomerDestinationManager.Instance.BarTableCount;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!CustomerDestinationManager.Instance.TryGetBarPoint(this, out currentBarTable, out currentSeat)) // is there any empty seat check
            {
                break;
            }

            if (SetDestinationOnNavMesh(currentSeat.position))
            {
                CurrentState = CustomerState.WalkingToTable; // if yes walks to table
                return true;
            }

            currentBarTable.ReleasePoint(this);
            currentBarTable = null;
            currentSeat = null;
        }

        return false;
    }

    private void UpdateWalkingToTable() // is npc reached to point?
    {
        if (HasReachedDestination())
        {
            CurrentState = CustomerState.PlayingBlackjack; // if yes npc stops and stars playing bj
            playTimer = 0f; 
            cheatTimer = Random.Range(minPlayBeforeCheat, minPlayAfterCheat);
            activityTimer = Random.Range(minActivityDuration, maxActivityDuration);
            agent.ResetPath();
        }
    }

    private void LeaveCasino() // leave casino
    {
        ReleaseCurrentDestination();

        CurrentState = CustomerState.Leaving; // npc starts walking to exit

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        if (CustomerDestinationManager.Instance.ExitPoint != null)
        {
            if (!SetDestinationOnNavMesh(CustomerDestinationManager.Instance.ExitPoint.position))
            {
                CurrentState = CustomerState.Gone;
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCurrentDestination()
    {
        if (currentTable != null)
        {
            currentTable.ReleaseSeat(this); // occupied seat reserved for another NPC
            currentTable = null;
        }

        if (currentSlotMachine != null)
        {
            currentSlotMachine.ReleaseSlot(this);
            currentSlotMachine = null;
        }

        if (currentRouletteTable != null)
        {
            currentRouletteTable.ReleaseSeat(this);
            currentRouletteTable = null;
        }

        if (currentBarTable != null)
        {
            currentBarTable.ReleasePoint(this);
            currentBarTable = null;
        }

        currentSeat = null;
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

      private bool TryPlaceOnNavMesh() // navmesh test
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

      private bool IsCasinoRunning()
      {
        return GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.CasinoFloor;
      }

      private bool SetDestinationOnNavMesh(Vector3 destination) // navmesh exit test
      {
          if (!agent.isOnNavMesh && !TryPlaceOnNavMesh())
          {
              return false;
          }

          if (NavMesh.SamplePosition(destination, out NavMeshHit hit, destinationSearchDistance, NavMesh.AllAreas))
          {
              NavMeshPath path = new NavMeshPath();
              if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
              {
                  agent.isStopped = false;
                  return agent.SetPath(path);
              }
          }

          Debug.LogWarning($"{name} could not find a NavMesh destination near {destination}.");
          return false;
      }

      private void UpdatePlayingBlackjack() 
      {
        playTimer += Time.deltaTime;
        if (IsCheater && playTimer >= cheatTimer) // if playTimer gets higher or equeal to cheatTimer that npc becomes suspicious
        {
            BecomeSuspicious();
            return;
        }

        if (playTimer >= activityTimer)
        {
            LeaveCasino();
        }
      }

      private void BecomeSuspicious()
      {
        CurrentState = CustomerState.Suspicious;
        agent.ResetPath();

        SuspicionIndicator indicator = GetComponentInChildren<SuspicionIndicator>(true);
        if(indicator != null)
        {
            indicator.Show();
        }
      }
      public void Interact()
      {
        StartInterrogation(); // interrogation start (yeşim bunu sen kullanacaksın)
      }
      public void StartInterrogation()
      {
        if (!CanBeInteractedWith || CurrentState != CustomerState.Suspicious)
        {
            return;
        }

        CanBeInteractedWith = false;
        CurrentState = CustomerState.Interrogating;

        ReleaseCurrentDestination();

        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        SuspicionIndicator indicator = GetComponentInChildren<SuspicionIndicator>(true);
        if (indicator != null)
        {
            indicator.Hide();
        }
      }
      // İbrahim'in sorgu UI sistemi sorgu bittikten sonra NPC'yi serbest bırakınca çağıracak
      public void ReleaseFromInterrogation() // if innocent go out
      {
        if (CurrentState != CustomerState.Interrogating)
        {
            return;
        }

        LeaveCasino();
      }
       // sorgu UI veya player mechanics NPC'yi arka odaya gönderince çağıracak
      public void SendToBackRoom() // backroom sending script (ibo burası sende UI sonucu için burayı alacaksın)
      {
        if (CurrentState != CustomerState.Interrogating && CurrentState != CustomerState.Suspicious)
        {
            return;
        }

        CanBeInteractedWith = false;
        CurrentState = CustomerState.EscortedToBackRoom;

        ReleaseCurrentDestination();

        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        SuspicionIndicator indicator = GetComponentInChildren<SuspicionIndicator>(true);
        if (indicator != null)
        {
            indicator.Hide();
        }
      }
    // hileci NPC yakalanmadan bırakılırsa çıkışa göndermek için çağrılacak
    public void MissCheater()
    {
        if (!IsCheater)
        {
            return;
        }

        CanBeInteractedWith = false;

        SuspicionIndicator indicator = GetComponentInChildren<SuspicionIndicator>(true);
        if (indicator != null)
        {
            indicator.Hide();
        }

        LeaveCasino();
    }
}
