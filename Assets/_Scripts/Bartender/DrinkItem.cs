using UnityEngine;
using System.Collections;

public class DrinkItem : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Press E to Drink";

    [Header("Yenilenme Ayarlarý")]
    [Tooltip("Bardak içildikten kaç saniye sonra masaya geri dönsün?")]
    public float respawnTime = 30f;

    private Collider itemCollider;
    private Renderer[] itemRenderers;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
        itemRenderers = GetComponentsInChildren<Renderer>();
    }

    public void Interact()
    {
        if (BartenderManager.Instance != null)
        {
            BartenderManager.Instance.DrinkCocktail();

            StartCoroutine(HideAndRespawnRoutine());
        }
    }

    private IEnumerator HideAndRespawnRoutine()
    {
        if (itemCollider != null) itemCollider.enabled = false; 

        foreach (var rnd in itemRenderers)
        {
            if (rnd != null) rnd.enabled = false; 
        }

        yield return new WaitForSeconds(respawnTime);

        if (itemCollider != null) itemCollider.enabled = true;

        foreach (var rnd in itemRenderers)
        {
            if (rnd != null) rnd.enabled = true;
        }
    }
}