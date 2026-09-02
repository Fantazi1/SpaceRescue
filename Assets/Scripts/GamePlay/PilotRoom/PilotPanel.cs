using UnityEngine;

public class PilotPanel : MonoBehaviour, IInteractable
{
    [Header("Текст підказки")]
    public string promptOn = "[E] Turn On Cabin Systems";
    public string promptOff = "[E] Turn Off Cabin Systems";

    [Header("Освітлення та Екрани")]
    [Tooltip("Об'єкти світла або екранів, які будуть вмикатися/вимикатися")]
    public GameObject[] lightSystems;

    [Header("Звук взаємодії")]
    [Tooltip("Короткий звук клацання тумблера")]
    public AudioSource clickSound;

    private bool isSystemsActive = false;

    void Start()
    {
        UpdateSystemsState();
    }

    public string GetInteractText()
    {
        return isSystemsActive ? promptOff : promptOn;
    }

    public void Interact()
    {
        isSystemsActive = !isSystemsActive;
        UpdateSystemsState();

        if (clickSound != null)
        {
            clickSound.Play();
        }
    }

    private void UpdateSystemsState()
    {
        if (lightSystems == null) return;

        foreach (var obj in lightSystems)
        {
            if (obj != null)
            {
                obj.SetActive(isSystemsActive);
            }
        }
    }
}