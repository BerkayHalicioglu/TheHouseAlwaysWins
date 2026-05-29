using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI Ayarları")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public GameObject interrogationUI;

    [Header("Sorgu Odası Işınlanma Noktaları")]
    public Transform playerInterrogationSpawn;
    public Transform npcInterrogationSpawn;

    [Header("Kamera Ayarları")]
    public GameObject mainCamera;
    public GameObject interrogationCamera;

    [Header("Player")]
    public PlayerController playerController;
    public PlayerLook playerLook;
    public GameObject playerObject;

    private Vector3 playerReturnPosition;
    private Quaternion playerReturnRotation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (interrogationUI != null)
            interrogationUI.SetActive(false);
    }

    private void OnEnable()
    {
        QTEManager.OnInterrogationQTESuccess += HandleInterrogationQTESuccess;
    }

    private void OnDisable()
    {
        QTEManager.OnInterrogationQTESuccess -= HandleInterrogationQTESuccess;
    }

    private void HandleInterrogationQTESuccess(CustomerAI target)
    {
        StartCoroutine(TransitionRoutine(target));
    }

    private IEnumerator TransitionRoutine(CustomerAI target)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        if (playerController != null)
            playerController.SetMovementEnabled(false);
        if (playerLook != null)
            playerLook.SetLookEnabled(false);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        if (playerObject != null)
        {
            playerReturnPosition = playerObject.transform.position;
            playerReturnRotation = playerObject.transform.rotation;
            playerObject.transform.position = playerInterrogationSpawn.position;
            playerObject.transform.rotation = playerInterrogationSpawn.rotation;
        }

        NavMeshAgent npcAgent = target.GetComponent<NavMeshAgent>();
        if (npcAgent != null) npcAgent.enabled = false;
        target.transform.position = npcInterrogationSpawn.position;
        target.transform.rotation = npcInterrogationSpawn.rotation;
        if (npcAgent != null) npcAgent.enabled = true;

        if (mainCamera != null) mainCamera.SetActive(false);
        if (interrogationCamera != null) interrogationCamera.SetActive(true);

        if (interrogationUI != null)
            interrogationUI.SetActive(true);

        InterrogationManager mgr = Object.FindFirstObjectByType<InterrogationManager>(FindObjectsInactive.Include);
        if (mgr != null)
        {
            mgr.gameObject.SetActive(true);
            mgr.SetTarget(target);
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] InterrogationManager sahnede bulunamadı.");
        }

        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToCasino(System.Action onComplete = null)
    {
        StartCoroutine(ReturnRoutine(onComplete));
    }

    private IEnumerator ReturnRoutine(System.Action onComplete = null)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        if (interrogationUI != null) interrogationUI.SetActive(false);
        if (interrogationCamera != null) interrogationCamera.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);

        if (playerObject != null)
        {
            playerObject.transform.position = playerReturnPosition;
            playerObject.transform.rotation = playerReturnRotation;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.SetMovementEnabled(true);
        if (playerLook != null)
            playerLook.SetLookEnabled(true);

        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        onComplete?.Invoke();
    }
}
