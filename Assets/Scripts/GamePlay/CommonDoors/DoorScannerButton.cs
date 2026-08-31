using UnityEngine;

public class DoorScannerButton : MonoBehaviour, IInteractable
{
    [Header("Текст підказки")]
    public string promptOpen = "[E] Open the door";
    public string promptClose = "[E] Close the door";

    [Header("Посилання на двері")]
    public SpaceDoorCommon doorController; 

    private bool isOpen = false;

    void Start()
    {
        if (doorController == null)
        {
            doorController = GetComponent<SpaceDoorCommon>();
            if (doorController == null)
            {
                doorController = Object.FindFirstObjectByType<SpaceDoorCommon>();
            }
        }
    }

    public string GetInteractText()
    {
        return isOpen ? promptClose : promptOpen;
    }

    public void Interact()
    {
        if (doorController != null)
        {
            isOpen = !isOpen;

            if (isOpen)
            {
                doorController.OpenDoor();
            }
            else
            {
                doorController.CloseDoor();
            }
        }
    }
}