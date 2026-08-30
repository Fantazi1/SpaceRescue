using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlaylistManager : MonoBehaviour
{
    [Header("Плейлист")]
    public AudioClip[] tracks;
    public bool randomOrder = false;

    private AudioSource audioSource;
    private int currentIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        audioSource.playOnAwake = false;

        if (tracks.Length > 0)
        {
            currentIndex = randomOrder ? Random.Range(0, tracks.Length) : 0;
            PlayTrack(currentIndex);
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && tracks.Length > 0)
        {
            if (randomOrder)
            {
                int lastIndex = currentIndex;

                while (currentIndex == lastIndex && tracks.Length > 1)
                {
                    currentIndex = Random.Range(0, tracks.Length);
                }
            }
            else
            {
                currentIndex = (currentIndex + 1) % tracks.Length;
            }

            PlayTrack(currentIndex);
        }
    }

    private void PlayTrack(int index)
    {
        audioSource.clip = tracks[index];
        audioSource.Play();
    }
}