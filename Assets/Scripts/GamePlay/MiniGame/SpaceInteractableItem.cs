using UnityEngine;
using UnityEngine.Audio;

public class SpaceInteractableItem : MonoBehaviour, IInteractable
{
    [Header("Налаштування")]
    public string promptMessage = "[E] Collect";
    public bool isValuableAsteroid = true;
    public int rewardAmount = 10;

    [Header("Аудіо")]
    public AudioClip[] pickupSounds;
    public AudioMixerGroup outputAudioGroup;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Випливаючий текст")]
    public GameObject floatingTextPrefab;
    public Color asteroidTextColor = Color.yellow;
    public Color animalTextColor = Color.green;

    public string GetInteractText()
    {
        return promptMessage;
    }

    public void Interact()
    {
        string messageText = isValuableAsteroid ? $"+{rewardAmount} $" : "+1 Animal";
        Color textColor = isValuableAsteroid ? asteroidTextColor : animalTextColor;

        PlayRandomSoundWithMixer();

        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ft = textObj.GetComponent<FloatingText>();
            if (ft != null)
            {
                ft.Setup(messageText, textColor);
            }
        }

        if (GameManager.Instance != null)
        {
            if (isValuableAsteroid)
            {
                GameManager.Instance.AddCurrency(rewardAmount); //
            }
            else
            {
                GameManager.Instance.AddAnimal(); //
            }
        }

        Destroy(gameObject);
    }

    private void PlayRandomSoundWithMixer()
    {
        if (pickupSounds == null || pickupSounds.Length == 0) return;

        int randomIndex = Random.Range(0, pickupSounds.Length);
        AudioClip selectedClip = pickupSounds[randomIndex];

        if (selectedClip == null) return;

        GameObject soundObj = new GameObject("TempPickupSound");
        soundObj.transform.position = transform.position;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = selectedClip;
        audioSource.volume = soundVolume;

        if (outputAudioGroup != null)
        {
            audioSource.outputAudioMixerGroup = outputAudioGroup;
        }

        audioSource.spatialBlend = 1f;
        audioSource.Play();

        Destroy(soundObj, selectedClip.length);
    }
}