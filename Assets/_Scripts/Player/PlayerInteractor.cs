using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask suspectLayer;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
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

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, suspectLayer ))
        {
            IInteractable interactable =
                hitInfo.collider.GetComponent<IInteractable>() ??
                hitInfo.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            
            }
            else
            {
                
            }
        }
        else
        {
            
        }
    }
    private void TryDealCards()
    {
        if (cameraTransform == null)
            return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance))
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