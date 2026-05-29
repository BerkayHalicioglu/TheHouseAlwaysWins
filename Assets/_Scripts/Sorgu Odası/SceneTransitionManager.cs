using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Interrogation")]
    [SerializeField] private GameObject interrogationUI;
    [SerializeField] private InterrogationManager interrogationManager;
    [SerializeField] private InterrogationView interrogationView;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerInterrogationSpawn;
    [SerializeField] private Transform npcInterrogationSpawn;

    [Header("Scene References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject interrogationCamera;

    private CustomerAI currentSuspect;
    private Vector3 playerReturnPosition;
    private Quaternion playerReturnRotation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        QTEManager.OnInterrogationQTESuccess += HandleQTESuccess;
        InterrogationManager.OnInterrogationComplete += TransitionBackToCasino;
    }

    private void OnDisable()
    {
        QTEManager.OnInterrogationQTESuccess -= HandleQTESuccess;
        InterrogationManager.OnInterrogationComplete -= TransitionBackToCasino;
    }

    private void HandleQTESuccess(CustomerAI suspect)
    {
        currentSuspect = suspect;
        StartCoroutine(TransitionToBackRoomRoutine());
    }

    private IEnumerator TransitionToBackRoomRoutine()
    {
        SetPlayerMovement(false);
        GameManager.Instance?.EnterBackRoom();

        playerReturnPosition = player.transform.position;
        playerReturnRotation = player.transform.rotation;

        yield return StartCoroutine(Fade(0f, 1f));

        MovePlayerTo(playerInterrogationSpawn.position, playerInterrogationSpawn.rotation);
        MoveNPCTo(currentSuspect, npcInterrogationSpawn.position, npcInterrogationSpawn.rotation);

        if (mainCamera != null) mainCamera.SetActive(false);
        if (interrogationCamera != null) interrogationCamera.SetActive(true);

        if (interrogationUI != null) interrogationUI.SetActive(true);
        interrogationManager?.SetSuspect(currentSuspect);
        interrogationView?.Show(currentSuspect);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void TransitionBackToCasino()
    {
        StartCoroutine(TransitionBackRoutine());
    }

    private IEnumerator TransitionBackRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f));

        if (interrogationUI != null) interrogationUI.SetActive(false);
        if (interrogationCamera != null) interrogationCamera.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);

        MovePlayerTo(playerReturnPosition, playerReturnRotation);

        if (currentSuspect != null)
        {
            Destroy(currentSuspect.gameObject);
            currentSuspect = null;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance?.ExitBackRoom();
        SetPlayerMovement(true);

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private void MovePlayerTo(Vector3 position, Quaternion rotation)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.SetPositionAndRotation(position, rotation);
        if (cc != null) cc.enabled = true;
    }

    private void MoveNPCTo(CustomerAI npc, Vector3 position, Quaternion rotation)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        npc.transform.SetPositionAndRotation(position, rotation);
        if (agent != null) agent.enabled = true;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeCanvasGroup == null) yield break;
        fadeCanvasGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = to;
        if (to == 0f) fadeCanvasGroup.blocksRaycasts = false;
    }

    private void SetPlayerMovement(bool enabled)
    {
        playerController?.SetMovementEnabled(enabled);
    }
}
