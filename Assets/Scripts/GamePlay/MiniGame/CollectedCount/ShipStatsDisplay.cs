using UnityEngine;
using TMPro;

public class ShipStatsDisplay : MonoBehaviour
{
    [Header("Текстові поля на стіні")]
    public TMP_Text currencyText;  
    public TMP_Text animalsText;  

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (currencyText != null)
            {
                currencyText.text = $"Balance: {GameManager.Instance.currency} $";
            }

            if (animalsText != null)
            {
                animalsText.text = $"Saved animals: {GameManager.Instance.savedAnimalsCount}";
            }
        }
    }
}