using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAudioManager : MonoBehaviour
{
    [Header("Джерело звуку")]
    [SerializeField] private AudioSource audioSource;

    [Header("Масиви звуків (випадковий вибір)")]
    [SerializeField] private AudioClip[] hoverClips;
    [SerializeField] private AudioClip[] clickClips;

    [Header("Окремий звук для виходу")]
    [SerializeField] private AudioClip quitClickClip;

    [Header("Посилання на кнопки")]
    [SerializeField] private Button[] menuButtons; // Всі звичайні кнопки
    [SerializeField] private Button quitButton;    // Окрема кнопка виходу

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 1. Підписуємо всі стандартні кнопки
        foreach (Button btn in menuButtons)
        {
            if (btn == null) continue;

            btn.onClick.AddListener(PlayRandomClickSound);
            AddHoverEvent(btn);
        }

        // 2. Підписуємо кнопку виходу (якщо вона призначена)
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(PlayQuitSound);
            AddHoverEvent(quitButton); // Звук наведення для неї залишаємо стандартним
        }
    }

    // Допоміжний метод, щоб не дублювати код створення EventTrigger
    private void AddHoverEvent(Button btn)
    {
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = btn.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { PlayRandomHoverSound(); });

        trigger.triggers.Add(hoverEntry);
    }

    private void PlayRandomHoverSound()
    {
        // Перевіряємо, чи є хоча б один звук у масиві
        if (audioSource != null && hoverClips.Length > 0)
        {
            int randomIndex = Random.Range(0, hoverClips.Length);
            audioSource.PlayOneShot(hoverClips[randomIndex]);
        }
    }

    private void PlayRandomClickSound()
    {
        if (audioSource != null && clickClips.Length > 0)
        {
            int randomIndex = Random.Range(0, clickClips.Length);
            audioSource.PlayOneShot(clickClips[randomIndex]);
        }
    }

    private void PlayQuitSound()
    {
        if (audioSource != null && quitClickClip != null)
        {
            audioSource.PlayOneShot(quitClickClip);
        }
    }
}