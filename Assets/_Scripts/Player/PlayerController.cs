using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;
    private bool movementEnabled = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 moveInput = Vector2.zero;

        if (movementEnabled && Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
        }

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        bool isSprinting = movementEnabled &&
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed &&
            (StaminaManager.Instance == null || StaminaManager.Instance.CanSprint);

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (isSprinting && StaminaManager.Instance != null)
            StaminaManager.Instance.DrainStamina(StaminaManager.Instance.SprintDrainPerSecond * Time.deltaTime);

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        Debug.Log($"[PLAYER DEBUG] Movement enabled set to: {movementEnabled} on {gameObject.name}");
    }
}