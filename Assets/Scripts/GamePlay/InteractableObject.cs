using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Налаштування взаємодії")]
    [Tooltip("Текст підказки, який з'явиться на екрані (наприклад: 'Повернутися на корабель' або 'Відчинити двері')")]
    public string promptMessage = "[E] Open the gateway";

    [Tooltip("Що станеться при натисканні (налаштовується в Інспекторі)")]
    public UnityEvent onInteract;

    public string GetInteractText()
    {
        return promptMessage;
    }

    public void Interact()
    {
        onInteract.Invoke();
    }
}