using UnityEngine;
using UnityEngine.InputSystem;

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
    public int spawnRateLevel = 1;

    [Header("Налаштування цін")]
    public int baseUpgradeCost = 200;

    [Header("Система трофеїв (Статуї на сцені)")]
    [Tooltip("Масив статуй, які вже розставлені на сцені (вони мають бути вимкнені на старті)")]
    public GameObject[] statues;
    [Tooltip("Скільки тварин треба врятувати для відкриття однієї статуї")]
    public int animalsPerStatue = 5;

    private int unlockedStatuesCount = 0;

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

        if (statues != null)
        {
            foreach (var statue in statues)
            {
                if (statue != null) statue.SetActive(false);
            }
        }
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

        int targetUnlocked = savedAnimalsCount / animalsPerStatue;

        while (unlockedStatuesCount < targetUnlocked && statues != null && unlockedStatuesCount < statues.Length)
        {
            if (statues[unlockedStatuesCount] != null)
            {
                statues[unlockedStatuesCount].SetActive(true);
                Debug.Log($"The statue has been restored №{unlockedStatuesCount + 1} at scene!");
            }
            unlockedStatuesCount++;
        }
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