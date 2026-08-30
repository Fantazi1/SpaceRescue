using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("Що станеться при натисканні (налаштовується в Інспекторі)")]
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract.Invoke();
    }
}