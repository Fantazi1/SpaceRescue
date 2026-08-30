using UnityEngine;

public class AstronautEffects : MonoBehaviour
{
    [Header("Зв'язок з рухом")]
    public AstronautMovement astronautMovement;

    [Header("Камера (Head Bobbing)")]
    public Transform playerCamera;
    public float walkBobSpeed = 12f;
    public float walkBobAmount = 0.08f;
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.02f;

    [Header("Звуки кроків")]
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;

    [Header("Звук дихання")]
    public AudioSource breathingSource;
    public AudioClip[] breathingSounds;

    private float defaultPosY = 0f;
    private float timer = 0f;
    private float footstepTimer = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        if (astronautMovement == null)
        {
            astronautMovement = GetComponent<AstronautMovement>();
        }

        if (playerCamera != null)
        {
            defaultPosY = playerCamera.localPosition.y;
        }

        lastPosition = transform.position;

        if (breathingSource != null)
        {
            breathingSource.loop = false;
        }
    }

    void Update()
    {
        HandleMovementEffects();
        HandleBreathing();
    }

    private void HandleMovementEffects()
    {
        if (playerCamera == null) return;

        Vector3 currentHorizontalPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastHorizontalPos = new Vector3(lastPosition.x, 0, lastPosition.z);
        float currentSpeed = Vector3.Distance(currentHorizontalPos, lastHorizontalPos) / Time.deltaTime;
        lastPosition = transform.position;

        float targetBob = 0f;

        if (astronautMovement != null && astronautMovement.isZeroGravity)
        {
            timer += Time.deltaTime * (idleBobSpeed * 0.5f);
            targetBob = Mathf.Sin(timer) * (idleBobAmount * 1.5f);
            footstepTimer = footstepInterval * 0.8f;
        }
        else if (currentSpeed > 0.1f)
        {
            timer += Time.deltaTime * walkBobSpeed;
            targetBob = Mathf.Sin(timer) * walkBobAmount;

            footstepTimer += Time.deltaTime * (currentSpeed / 5f);
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            timer += Time.deltaTime * idleBobSpeed;
            targetBob = Mathf.Sin(timer) * idleBobAmount;
            footstepTimer = footstepInterval * 0.8f;
        }

        playerCamera.localPosition = new Vector3(
            playerCamera.localPosition.x,
            Mathf.Lerp(playerCamera.localPosition.y, defaultPosY + targetBob, Time.deltaTime * 10f),
            playerCamera.localPosition.z
        );
    }

    private void HandleBreathing()
    {
        if (breathingSource != null && breathingSounds.Length > 0)
        {
            if (!breathingSource.isPlaying)
            {
                int rand = Random.Range(0, breathingSounds.Length);
                breathingSource.clip = breathingSounds[rand];
                breathingSource.Play();
            }
        }
    }

    private void PlayFootstep()
    {
        if (footstepSource == null || footstepSounds.Length == 0) return;

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        int rand = Random.Range(0, footstepSounds.Length);
        footstepSource.PlayOneShot(footstepSounds[rand]);
    }
}