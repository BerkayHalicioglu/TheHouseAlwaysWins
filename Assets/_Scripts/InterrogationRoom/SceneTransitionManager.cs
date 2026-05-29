using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI Ayarlar�")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    [Tooltip("Sorgu odas�na ge�ildi�inde a��lacak D�v/Serbest B�rak men�s�")]
    public GameObject interrogationUI; 

    [Header("Sorgu Odas� I��nlanma Noktalar�")]
    public Transform playerInterrogationSpawn;
    public Transform npcInterrogationSpawn;

    [Header("Kamera Ayarlar�")]
    public GameObject mainCamera;
    public GameObject interrogationCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTransitionToInterrogation(GameObject player, GameObject targetNPC)
    {
        StartCoroutine(TransitionRoutine(player, targetNPC));
    }

    private IEnumerator TransitionRoutine(GameObject player, GameObject targetNPC)
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

        player.transform.position = playerInterrogationSpawn.position;
        player.transform.rotation = playerInterrogationSpawn.rotation;

        NavMeshAgent npcAgent = targetNPC.GetComponent<NavMeshAgent>();
        if (npcAgent != null) npcAgent.enabled = false;

        targetNPC.transform.position = npcInterrogationSpawn.position;
        targetNPC.transform.rotation = npcInterrogationSpawn.rotation;

        if (npcAgent != null) npcAgent.enabled = true;

        if (mainCamera != null) mainCamera.SetActive(false);
        if (interrogationCamera != null) interrogationCamera.SetActive(true);

        if (interrogationUI != null) interrogationUI.SetActive(true);

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
    }
}