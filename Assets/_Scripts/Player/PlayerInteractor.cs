using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask suspectLayer;
    [SerializeField] private LayerMask tableLayer;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (suspectLayer.value == 0)
        {
            Debug.LogWarning("[PlayerInteractor] suspectLayer is not assigned. E interaction will not detect suspects.");
        }

        if (tableLayer.value == 0)
        {
            Debug.LogWarning("[PlayerInteractor] tableLayer is not assigned. F interaction will not detect game tables.");
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryInteract();

        if (Keyboard.current.fKey.wasPressedThisFrame)
            TryDealCards();
    }

    private void TryInteract()
    {
        if (cameraTransform == null)
            return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, suspectLayer))
        {
            IInteractable interactable =
                hitInfo.collider.GetComponent<IInteractable>() ??
                hitInfo.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void TryDealCards()
    {
        if (cameraTransform == null)
            return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, tableLayer))
        {
            ICardDealable cardDealable =
                hitInfo.collider.GetComponent<ICardDealable>() ??
                hitInfo.collider.GetComponentInParent<ICardDealable>();

            if (cardDealable != null)
            {
                cardDealable.DealCards();
            }
        }
    }
}