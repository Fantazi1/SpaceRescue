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

    [Header("Блокування гравця")]
    public MonoBehaviour cameraScript;
    public MonoBehaviour movementScript;

    [Header("Тексти інформації (Назва та Рівень)")]
    public TMP_Text zoneInfoText;
    public TMP_Text speedInfoText;
    public TMP_Text luckInfoText;

    [Header("Тексти цін")]
    public TMP_Text zonePriceText;
    public TMP_Text speedPriceText;
    public TMP_Text luckPriceText;

    [Header("Текст балансу")]
    public TMP_Text currencyText;

    [Header("Кнопки покупки")]
    public Button buyZoneBtn;
    public Button buySpeedBtn;
    public Button buyLuckBtn;

    void Start()
    {
        IsOpen = false;
        if (upgradeCanvas != null) upgradeCanvas.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(CloseMenu);

        if (buyZoneBtn != null) buyZoneBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.zoneLevel));
        if (buySpeedBtn != null) buySpeedBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.speedLevel));
        if (buyLuckBtn != null) buyLuckBtn.onClick.AddListener(() => Buy(ref GameManager.Instance.luckLevel));
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
    }

    public void CloseMenu()
    {
        IsOpen = false;
        upgradeCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraScript != null) cameraScript.enabled = true;
        if (movementScript != null) movementScript.enabled = true;
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