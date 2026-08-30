using UnityEngine;

public class AirlockPanel : MonoBehaviour, IInteractable
{
    [Header("Текст підказки")]
    public string promptMessage = "[E] Open the gateway";

    private AirlockAlarmController alarm;

    void Start()
    {
        alarm = Object.FindFirstObjectByType<AirlockAlarmController>();
    }

    // Повертаємо текст підказки для PlayerInteractor
    public string GetInteractText()
    {
        return promptMessage;
    }

    public void Interact()
    {
        if (alarm != null)
        {
            alarm.ToggleAirlock();
        }
    }
}