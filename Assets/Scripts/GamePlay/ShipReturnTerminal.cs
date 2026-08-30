using System.Collections;
using UnityEngine;
using TMPro;

public class ShipReturnTerminal : MonoBehaviour, IInteractable
{
    [Header("Текст підказки")]
    public string promptMessage = "Повернутися на корабель";

    [Header("Куди телепортувати (Всередину корабля)")]
    public Transform shipSpawnPoint;

    [Header("Налаштування затемнення")]
    public CanvasGroup fadeScreen;
    public float fadeSpeed = 2f;
    public float blackScreenDuration = 0.5f;

    private static bool isTeleporting = false;

    public string GetInteractText()
    {
        PlayerInteractor interactor = Object.FindFirstObjectByType<PlayerInteractor>();
        if (interactor != null)
        {
            System.Reflection.FieldInfo field = typeof(PlayerInteractor).GetField("promptText");
            if (field != null)
            {
                TMP_Text t = field.GetValue(interactor) as TMP_Text;
                if (t != null) t.text = promptMessage;
            }
        }

        return promptMessage;
    }

    public void Interact()
    {
        if (isTeleporting) return;

        CharacterController cc = Object.FindFirstObjectByType<CharacterController>();
        if (cc != null)
        {
            AstronautMovement movement = cc.GetComponent<AstronautMovement>();
            StartCoroutine(TeleportSequence(cc, movement));
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