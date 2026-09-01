using UnityEngine;
using UnityEngine.InputSystem; // Додано для обробки ESC

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Статистика")]
    public int currency = 0;
    public int savedAnimalsCount = 0;

    [Header("Покращення")]
    public int zoneLevel = 1;
    public int speedLevel = 1;
    public int luckLevel = 1;

    [Header("Налаштування цін")]
    public int baseUpgradeCost = 200;

    [Header("Пауза")]
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    public MonoBehaviour cameraScript;
    public MonoBehaviour movementScript;

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

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (UpgradeMenuController.IsOpen) return;

            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraScript != null) cameraScript.enabled = true;
        if (movementScript != null) movementScript.enabled = true;
    }

    public void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraScript != null) cameraScript.enabled = false;
        if (movementScript != null) movementScript.enabled = false;
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

    public int GetCost(int level) => level * baseUpgradeCost;

    public bool TryBuyUpgrade(ref int upgradeLevel)
    {
        int cost = GetCost(upgradeLevel);
        if (currency >= cost)
        {
            currency -= cost;
            upgradeLevel++;
            return true;
        }
        return false;
    }
}