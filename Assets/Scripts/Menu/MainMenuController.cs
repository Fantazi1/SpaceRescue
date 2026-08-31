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

    [Header("Ефекти при старті гри")]
    public ParticleSystem[] menuParticles;
    public Color targetParticleColor = Color.cyan;

    [Header("Динаміка партіклів")]
    public float particleSpeedMultiplier = 4f;   
    public float particleEmissionMultiplier = 5f; 

    public Transform shipTransform;
    public float forwardDistance = 10f;
    public float startTransitionDuration = 1.5f;

    [Header("Керування напрямком польоту")]
    public Vector3 flightDirection = new Vector3(0, 0, 1);

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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (menuParticles != null)
        {
            foreach (var ps in menuParticles)
            {
                if (ps != null)
                {
                    var mainModule = ps.main;
                    mainModule.startColor = targetParticleColor;
                    mainModule.startSpeedMultiplier *= particleSpeedMultiplier;

                    var emissionModule = ps.emission;
                    emissionModule.rateOverTimeMultiplier *= particleEmissionMultiplier;

                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
                    int aliveCount = ps.GetParticles(particles);
                    for (int i = 0; i < aliveCount; i++)
                    {
                        particles[i].startColor = targetParticleColor;
                        particles[i].velocity *= particleSpeedMultiplier;
                    }
                    ps.SetParticles(particles, aliveCount);

                    ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
                    if (psr != null && psr.material != null)
                    {
                        if (psr.material.HasProperty("_BaseColor"))
                            psr.material.SetColor("_BaseColor", targetParticleColor);
                        if (psr.material.HasProperty("_TintColor"))
                            psr.material.SetColor("_TintColor", targetParticleColor);
                        if (psr.material.HasProperty("_EmissionColor"))
                            psr.material.SetColor("_EmissionColor", targetParticleColor * 2f);
                    }
                }
            }
        }

        if (shipTransform != null)
        {
            Vector3 startPos = shipTransform.position;
            Vector3 endPos = startPos + (flightDirection.normalized * forwardDistance);

            float elapsed = 0f;
            while (elapsed < startTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / startTransitionDuration;

                shipTransform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(startTransitionDuration);
        }

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