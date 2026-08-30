using System.Collections;
using UnityEngine;

public class AirlockAlarmController : MonoBehaviour
{
    [Header("Налаштування тривоги")]
    public float alarmDuration = 5f;

    [Header("Звук")]
    public AudioSource alarmAudioSource;

    [Header("Світло")]
    public Light[] staticAlarmLights;
    public Light[] rotatingSpotlights;
    public float rotationSpeed = 150f;

    private bool isAlarmActive = false;
    private bool isDoorOpen = false; 

    void Start()
    {
        foreach (Light l in staticAlarmLights) if (l != null) l.enabled = false;
        foreach (Light s in rotatingSpotlights) if (s != null) s.enabled = false;
    }

    public void ToggleAirlock()
    {
        if (isAlarmActive) return; 

        if (!isDoorOpen)
        {
            StartCoroutine(AlarmRoutine());
        }
        else
        {
            SpaceDoor[] doors = Object.FindObjectsByType<SpaceDoor>(FindObjectsSortMode.None);
            foreach (SpaceDoor door in doors)
            {
                door.CloseDoor();
            }
            isDoorOpen = false;
        }
    }

    private IEnumerator AlarmRoutine()
    {
        isAlarmActive = true;

        if (alarmAudioSource != null) alarmAudioSource.Play();
        foreach (Light l in staticAlarmLights) if (l != null) l.enabled = true;
        foreach (Light s in rotatingSpotlights) if (s != null) s.enabled = true;

        yield return new WaitForSeconds(alarmDuration);

        isAlarmActive = false;
        if (alarmAudioSource != null) alarmAudioSource.Stop();
        foreach (Light l in staticAlarmLights) if (l != null) l.enabled = false;
        foreach (Light s in rotatingSpotlights) if (s != null) s.enabled = false;

        SpaceDoor[] doors = Object.FindObjectsByType<SpaceDoor>(FindObjectsSortMode.None);
        foreach (SpaceDoor door in doors)
        {
            door.OpenDoor();
        }

        isDoorOpen = true; 
    }

    void Update()
    {
        if (isAlarmActive)
        {
            foreach (Light s in rotatingSpotlights)
            {
                if (s != null)
                {
                    s.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
                }
            }
        }
    }
}