using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Статистика")]
    public int currency = 0;
    public int savedAnimalsCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log($"Balance: {currency}");
    }

    public void AddAnimal()
    {
        savedAnimalsCount++;
        Debug.Log($"Saved animals: {savedAnimalsCount}");
    }
}