using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AstronautMovement : MonoBehaviour
{
    [Header("Налаштування руху")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;

    [Tooltip("Увімкніть для польотів у космосі")]
    public bool isZeroGravity = false;

    [Header("Налаштування камери")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 80f;

    [Header("Посилання на прокачку")]
    public UpgradeMenuController upgradeMenu;

    public Vector3 velocity;
    private CharacterController controller;
    private float cameraRotationX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (upgradeMenu == null)
        {
            upgradeMenu = Object.FindFirstObjectByType<UpgradeMenuController>();
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        if (Time.timeScale == 0f || Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;

        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

        if (isZeroGravity)
        {
            float currentFlightSpeed = (upgradeMenu != null) ? upgradeMenu.GetCurrentSpeed() : walkSpeed;

            Vector3 move = playerCamera.right * inputDir.x + playerCamera.forward * inputDir.z;
            controller.Move(move * currentFlightSpeed * Time.deltaTime);

            velocity = Vector3.zero;
        }
        else
        {
            Vector3 move = transform.right * inputDir.x + transform.forward * inputDir.z;
            controller.Move(move * walkSpeed * Time.deltaTime);

            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}