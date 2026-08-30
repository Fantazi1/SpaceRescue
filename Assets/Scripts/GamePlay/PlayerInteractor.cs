using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
}

public class PlayerInteractor : MonoBehaviour
{
    [Header("Налаштування взаємодії")]
    public Camera playerCamera;
    public float interactRange = 3f;

    [Header("UI Підказка")]
    public GameObject promptUI;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (promptUI != null) promptUI.SetActive(true);

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact();
                }
            }
            else
            {
                if (promptUI != null) promptUI.SetActive(false);
            }
        }
        else
        {
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}