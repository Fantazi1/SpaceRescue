using UnityEngine;

public class AirlockPanel : MonoBehaviour, IInteractable
{
    private AirlockAlarmController alarm;

    void Start()
    {
        alarm = Object.FindFirstObjectByType<AirlockAlarmController>();
    }

    public void Interact()
    {
        if (alarm != null)
        {
            alarm.ToggleAirlock();
        }
    }
}