using UnityEngine;

public class SpaceDoorCommon : MonoBehaviour
{
    [Header("Частини дверей")]
    public Transform leftDoorPart;
    public Transform rightDoorPart;

    [Header("Налаштування руху")]
    public float openDistance = 3f;
    public float openSpeed = 2f;

    [Header("Звуки")]
    public AudioSource audioSource;    
    public AudioClip openSound;      
    public AudioClip closeSound;     

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpen = false;
    private bool previousIsOpenState = false; 

    void Start()
    {
        if (leftDoorPart != null)
        {
            leftClosedPos = leftDoorPart.localPosition;
            leftOpenPos = leftClosedPos + new Vector3(-openDistance, 0, 0);
        }

        if (rightDoorPart != null)
        {
            rightClosedPos = rightDoorPart.localPosition;
            rightOpenPos = rightClosedPos + new Vector3(openDistance, 0, 0);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            PlaySound(openSound);
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            PlaySound(closeSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void Update()
    {
        if (leftDoorPart != null)
        {
            Vector3 targetLeft = isOpen ? leftOpenPos : leftClosedPos;
            leftDoorPart.localPosition = Vector3.MoveTowards(leftDoorPart.localPosition, targetLeft, openSpeed * Time.deltaTime);
        }

        if (rightDoorPart != null)
        {
            Vector3 targetRight = isOpen ? rightOpenPos : rightClosedPos;
            rightDoorPart.localPosition = Vector3.MoveTowards(rightDoorPart.localPosition, targetRight, openSpeed * Time.deltaTime);
        }
    }
}