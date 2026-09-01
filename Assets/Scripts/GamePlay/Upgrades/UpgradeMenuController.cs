using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UpgradeMenuController : MonoBehaviour, IInteractable
{
    public static bool IsOpen { get; private set; }

    [Header("UI та Взаємодія")]
    public string promptMessage = "[E] Open the Upgrade Terminal";
    public GameObject upgradeCanvas;
    public Button closeButton;

    [Header("Анімація появи")]
    [Tooltip("CanvasGroup на полотні меню (для плавного прозорого затухання)")]
    public CanvasGroup canvasGroup;
    [Tooltip("Швидкість анімації появи/зникнення (чим більше, тим швидше)")]
    public float animSpeed = 12f;

    [Header("Блокування гравця")]
    public MonoBehaviour cameraScript;
    public MonoBehaviour movementScript;

    [Header("Тексти інформації (Назва та Рівень)")]
    public TMP_Text zoneInfoText;
    public TMP_Text speedInfoText;
    public TMP_Text luckInfoText;
    public TMP_Text spawnRateInfoText;

    [Header("Тексти цін")]
    public TMP_Text zonePriceText;
    public TMP_Text speedPriceText;
    public TMP_Text luckPriceText;
    public TMP_Text spawnRatePriceText;

    [Header("Текст балансу")]
    public TMP_Text currencyText;

    [Header("Кнопки покупки")]
    public Button buyZoneBtn;
    public Button buySpeedBtn;
    public Button buyLuckBtn;
    public Button buySpawnRateBtn;

    [Header("Баланс та налаштування ефектів (Inspector)")]
    public float speedBonusPerLevel = 5f;
    public float baseSpeedValue = 10f;
    public float luckBonusPerLevel = 2f;
    public float baseLuckValue = 5f;

    private Coroutine currentAnimRoutine;

    public float GetCurrentSpeed()
    {
        int level = GameManager.Instance != null ? GameManager.Instance.speedLevel : 1;
        return baseSpeedValue + ((level - 1) * speedBonusPerLevel);
    }

    public float GetCurrentLuck()
    {
        int level = GameManager.Instance != null ? GameManager.Instance.luckLevel : 1;
        return baseLuckValue + ((level - 1) * luckBonusPerLevel);
    }

    void Start()
    {
        IsOpen = false;
        if (upgradeCanvas != null)
        {
            upgradeCanvas.SetActive(false);
            if (canvasGroup == null)
            {
                canvasGroup = upgradeCanvas.GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = upgradeCanvas.AddComponent<CanvasGroup>();
            }
        }

        if (closeButton != null) closeButton.onClick.AddListener(CloseMenu);

        if (buyZoneBtn != null) buyZoneBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.zoneLevel));
        if (buySpeedBtn != null) buySpeedBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.speedLevel));
        if (buyLuckBtn != null) buyLuckBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.luckLevel));
        if (buySpawnRateBtn != null) buySpawnRateBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.spawnRateLevel));
    }

    void Update()
    {
        if (upgradeCanvas != null && upgradeCanvas.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseMenu();
            }
        }
    }

    public string GetInteractText()
    {
        return (upgradeCanvas != null && upgradeCanvas.activeSelf) ? "" : promptMessage;
    }

    public void Interact()
    {
        if (upgradeCanvas != null && !upgradeCanvas.activeSelf)
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        IsOpen = true;
        upgradeCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraScript != null) cameraScript.enabled = false;
        if (movementScript != null) movementScript.enabled = false;

        UpdateUI();

        Canvas.ForceUpdateCanvases();
        RectTransform canvasRect = upgradeCanvas.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRect);
        }

        if (currentAnimRoutine != null) StopCoroutine(currentAnimRoutine);
        currentAnimRoutine = StartCoroutine(AnimateMenu(true));
    }

    public void CloseMenu()
    {
        IsOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraScript != null) cameraScript.enabled = true;
        if (movementScript != null) movementScript.enabled = true;

        if (currentAnimRoutine != null) StopCoroutine(currentAnimRoutine);
        currentAnimRoutine = StartCoroutine(AnimateMenu(false));
    }

    private IEnumerator AnimateMenu(bool isOpen)
    {
        float targetAlpha = isOpen ? 1f : 0f;
        Vector3 targetScale = isOpen ? Vector3.one : new Vector3(0.85f, 0.85f, 0.85f);

        RectTransform rectTransform = upgradeCanvas.GetComponent<RectTransform>();

        if (isOpen)
        {
            canvasGroup.alpha = 0f;
            if (rectTransform != null) rectTransform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
        }

        while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.01f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * animSpeed);
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * animSpeed);
            }
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (rectTransform != null) rectTransform.localScale = targetScale;

        if (!isOpen)
        {
            upgradeCanvas.SetActive(false);
        }
    }

    private void Buy(ref int levelToUpgrade)
    {
        if (GameManager.Instance != null && GameManager.Instance.TryBuyUpgrade(ref levelToUpgrade))
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        currencyText.text = $"Balance: {GameManager.Instance.currency}";
        currencyText.ForceMeshUpdate();

        UpdateBtnUI(buyZoneBtn, zoneInfoText, zonePriceText, "Space flight area", GameManager.Instance.zoneLevel);
        UpdateBtnUI(buySpeedBtn, speedInfoText, speedPriceText, "Void travel speed", GameManager.Instance.speedLevel);
        UpdateBtnUI(buyLuckBtn, luckInfoText, luckPriceText, "Rare asteroid rate", GameManager.Instance.luckLevel);
        UpdateBtnUI(buySpawnRateBtn, spawnRateInfoText, spawnRatePriceText, "Spawn Frequency", GameManager.Instance.spawnRateLevel);
    }

    private void UpdateBtnUI(Button btn, TMP_Text infoText, TMP_Text priceText, string name, int level)
    {
        int cost = GameManager.Instance.GetCost(level);

        if (infoText != null)
        {
            infoText.text = $"{name} (Level {level})";
            infoText.ForceMeshUpdate();
        }

        if (priceText != null)
        {
            priceText.text = $"Price: {cost}";
            priceText.ForceMeshUpdate();
        }

        if (btn != null) btn.interactable = GameManager.Instance.currency >= cost;
    }
}