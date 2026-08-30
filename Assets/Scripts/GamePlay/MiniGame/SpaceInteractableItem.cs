using UnityEngine;

public class SpaceInteractableItem : MonoBehaviour, IInteractable
{
    [Header("Налаштування")]
    public string promptMessage = "[E] Collect";
    public bool isValuableAsteroid = true;
    public int rewardAmount = 10; 

    public string GetInteractText()
    {
        return promptMessage;
    }

    public void Interact()
    {
        if (GameManager.Instance != null)
        {
            if (isValuableAsteroid)
            {
                GameManager.Instance.AddCurrency(rewardAmount);
            }
            else
            {
                GameManager.Instance.AddAnimal();
            }
        }

        Destroy(gameObject);
    }
}