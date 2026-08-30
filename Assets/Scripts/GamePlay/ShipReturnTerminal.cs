using System.Collections;
using UnityEngine;

public class ShipReturnTerminal : MonoBehaviour, IInteractable
{
    [Header("Куди телепортувати (Всередину корабля)")]
    public Transform shipSpawnPoint;

    [Header("Налаштування затемнення")]
    public CanvasGroup fadeScreen;
    public float fadeSpeed = 2f;
    public float blackScreenDuration = 0.5f;

    private static bool isTeleporting = false;

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
        player.transform.position = shipSpawnPoint.position + new Vector3(0, 0.5f, 0);
        player.transform.rotation = shipSpawnPoint.rotation;

        if (movement != null)
        {
            movement.isZeroGravity = false;
            movement.velocity = Vector3.zero;
        }

        ProceduralWalk procWalk = player.GetComponent<ProceduralWalk>();
        if (procWalk != null)
        {
            procWalk.enabled = true;
        }

        player.enabled = true;

        SpaceDoor[] doors = Object.FindObjectsByType<SpaceDoor>(FindObjectsSortMode.None);
        foreach (SpaceDoor door in doors)
        {
            door.CloseDoor();
        }

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