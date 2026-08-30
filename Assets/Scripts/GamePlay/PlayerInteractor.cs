using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public interface IInteractable
{
    void Interact();
    string GetInteractText();
}

public class PlayerInteractor : MonoBehaviour
{
    [Header("Налаштування взаємодії")]
    public Camera playerCamera;
    public float interactRange = 3f;

    [Header("UI Підказка")]
    public GameObject promptUI;
    public TMP_Text promptText;

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
            ShipReturnTerminal returnTerminal = hit.collider.GetComponent<ShipReturnTerminal>();
            if (returnTerminal != null)
            {
                if (promptUI != null) promptUI.SetActive(true);

                if (promptText != null)
                {
                    promptText.text = returnTerminal.promptMessage;
                }

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    returnTerminal.Interact();
                }
                return;
            }

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (promptUI != null) promptUI.SetActive(true);

                if (promptText != null)
                {
                    promptText.text = interactable.GetInteractText();
                }

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact();
                }
            }
            else
            {
                HidePrompt();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    private void HidePrompt()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }
}