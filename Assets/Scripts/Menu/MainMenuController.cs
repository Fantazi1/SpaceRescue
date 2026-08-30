using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Налаштування сцени")]
    public string gameSceneName = "GameScene";

    [Header("Посилання на системи")]
    public MenuCamera menuCamera;

    public AstronautAnimator mechanicAstronaut;
    public AstronautAnimator goodbyeAstronaut;

    [Header("UI Панелі")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Кнопки меню")]
    public Button playButton;
    public Button settingsButton;
    public Button closeSettingsButton;
    public Button quitButton;

    private void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (playButton != null) playButton.onClick.AddListener(StartGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        menuCamera.SetTarget(menuCamera.pos2Settings);
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        menuCamera.SetTarget(menuCamera.pos1Menu);
    }

    public void OnSettingsChanged()
    {
        if (mechanicAstronaut != null) mechanicAstronaut.TriggerAction();
    }

    private void QuitGame()
    {
        mainMenuPanel.SetActive(false);
        menuCamera.SetTarget(menuCamera.pos3Exit);

        if (goodbyeAstronaut != null) goodbyeAstronaut.TriggerAction();

        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}