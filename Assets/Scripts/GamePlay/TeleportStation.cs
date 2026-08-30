using System.Collections;
using UnityEngine;

public class TeleportStation : MonoBehaviour, IInteractable
{
    [Header("Куди телепортувати")]
    public Transform destinationPoint;

    [Header("Налаштування невагомості")]
    [Tooltip("Поставте галочку, якщо це телепорт у відкритий космос")]
    public bool setZeroGravity = false;

    [Header("Налаштування затемнення")]
    public CanvasGroup fadeScreen;
    public float fadeSpeed = 2f;
    public float blackScreenDuration = 0.5f;

    private static bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        PlayerInteractor interactor = other.GetComponent<PlayerInteractor>();
        AstronautMovement movement = other.GetComponent<AstronautMovement>();

        if (cc != null && interactor != null && !isTeleporting)
        {
            StartCoroutine(TeleportSequence(cc, movement));
        }
    }

    public void Interact()
    {
        if (isTeleporting) return;

        PlayerInteractor player = Object.FindFirstObjectByType<PlayerInteractor>();
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            AstronautMovement movement = player.GetComponent<AstronautMovement>();

            if (cc != null)
            {
                StartCoroutine(TeleportSequence(cc, movement));
            }
        }
    }

    private IEnumerator TeleportSequence(CharacterController player, AstronautMovement movement)
    {
        isTeleporting = true;

        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            while (fadeScreen.alpha < 1f)
            {
                fadeScreen.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        player.enabled = false;
        player.transform.position = destinationPoint.position + new Vector3(0, 0.5f, 0);
        player.transform.rotation = destinationPoint.rotation;

        if (movement != null)
        {
            movement.isZeroGravity = setZeroGravity;
            movement.velocity = Vector3.zero;
            Debug.Log("Стан невагомості гравця змінено на: " + setZeroGravity);
        }
        else
        {
            Debug.LogError("Помилка: Телепорт не зміг знайти скрипт AstronautMovement на гравці!");
        }

        player.enabled = true;

        yield return new WaitForSeconds(blackScreenDuration);

        if (fadeScreen != null)
        {
            while (fadeScreen.alpha > 0f)
            {
                fadeScreen.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            fadeScreen.gameObject.SetActive(false);
        }

        isTeleporting = false;
    }
}