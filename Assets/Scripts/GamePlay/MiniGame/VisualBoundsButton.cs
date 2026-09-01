using System.Collections;
using UnityEngine;

public class VisualBoundsButton : MonoBehaviour, IInteractable
{
    [Header("Посилання на межу")]
    public SpaceBounds spaceBounds;

    [Header("Анімація кнопки")]
    [Tooltip("Дочірній об'єкт або сама модель кнопки, яка буде рухатися при натисканні")]
    public Transform buttonPart;
    [Tooltip("Зміщення локальної позиції при втисканні (наприклад, по осі Y або Z)")]
    public Vector3 pressOffset = new Vector3(0f, -0.05f, 0f);
    [Tooltip("Швидкість (тривалість) втискання та відпускання")]
    public float pressDuration = 0.15f;

    private Vector3 initialLocalPos;
    private bool isAnimating = false;

    void Start()
    {
        if (spaceBounds == null)
        {
            spaceBounds = Object.FindFirstObjectByType<SpaceBounds>();
        }

        if (buttonPart != null)
        {
            initialLocalPos = buttonPart.localPosition;
        }
        else
        {
            initialLocalPos = transform.localPosition;
            buttonPart = transform;
        }
    }

    public string GetInteractText()
    {
        if (spaceBounds == null) return "[E] Toggle Visual Bounds";

        // Динамічний текст підказки залежно від поточного стану
        bool isEnabled = spaceBounds.isVisualEnabled;
        return isEnabled ? "[E] Turn Visual OFF" : "[E] Turn Visual ON";
    }

    public void Interact()
    {
        if (spaceBounds == null || isAnimating) return;

        spaceBounds.isVisualEnabled = !spaceBounds.isVisualEnabled;

        StartCoroutine(PressAnimation());
    }

    private IEnumerator PressAnimation()
    {
        isAnimating = true;
        float elapsed = 0f;
        Vector3 targetPos = initialLocalPos + pressOffset;

        while (elapsed < pressDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pressDuration;
            buttonPart.localPosition = Vector3.Lerp(initialLocalPos, targetPos, t);
            yield return null;
        }
        buttonPart.localPosition = targetPos;

        elapsed = 0f;

        while (elapsed < pressDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pressDuration;
            buttonPart.localPosition = Vector3.Lerp(targetPos, initialLocalPos, t);
            yield return null;
        }
        buttonPart.localPosition = initialLocalPos;

        isAnimating = false;
    }
}