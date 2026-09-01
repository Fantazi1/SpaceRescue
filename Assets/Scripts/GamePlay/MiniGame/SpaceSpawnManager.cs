using System.Collections;
using UnityEngine;

public class SpaceSpawnManager : MonoBehaviour
{
    [Header("Префаби")]
    public GameObject normalAsteroidPrefab;
    public GameObject valuableAsteroidPrefab;

    [Header("Масив рідкісних тваринок")]
    public GameObject[] rareAnimalPrefabs;

    [Header("Налаштування спавну та інтервалу")]
    public Transform playerTransform;
    public AstronautMovement astronautMovement;
    public UpgradeMenuController upgradeMenu;
    public float spawnRadius = 30f;
    public float minSpawnDistance = 12f;

    [Header("Налаштування часу спавну (Баланс)")]
    [Tooltip("Базовий інтервал між спавнами на 1 рівні (в секундах)")]
    public float baseSpawnInterval = 2f;
    [Tooltip("Скільки секунд віднімається від інтервалу за кожний новий рівень")]
    public float intervalReductionPerLevel = 0.2f;
    [Tooltip("Мінімально можливий інтервал (нижче цієї межі час спавну не впаде ніколи)")]
    public float minAllowedInterval = 0.5f;

    [Header("Базові шанси спавну (на 1 рівні удачі)")]
    [Range(0f, 100f)] public float baseNormalChance = 60f;
    [Range(0f, 100f)] public float baseValuableChance = 30f;
    [Range(0f, 100f)] public float baseRareChance = 10f;

    [Header("Бонус від прокачки удачі")]
    [Tooltip("Скільки відсотків додається до цінних речей/тваринок за кожний новий рівень удачі")]
    public float luckBonusMultiplier = 1.5f;

    [Header("Обмеження шансів (Баланс в Inspector)")]
    [Tooltip("Максимально можливий шанс цінних астероїдів при прокачці")]
    public float maxValuableChanceLimit = 60f;
    [Tooltip("Максимально можливий шанс рідкісних тваринок при прокачці")]
    public float maxRareChanceLimit = 30f;
    [Tooltip("Мінімально можливий шанс звичайних астероїдів (щоб вони повністю не зникали)")]
    public float minNormalChanceLimit = 10f;

    [Header("Розмір: Звичайні астероїди")]
    public float normalMinSize = 1f;
    public float normalMaxSize = 1.8f;

    [Header("Розмір: Цінні астероїди")]
    public float valuableMinSize = 0.8f;
    public float valuableMaxSize = 1.2f;

    [Header("Розмір: Рідкісні тваринки")]
    public float animalMinSize = 0.5f;
    public float animalMaxSize = 1.0f;

    private float timer;

    private float CurrentSpawnInterval
    {
        get
        {
            int spawnLevel = (GameManager.Instance != null) ? GameManager.Instance.spawnRateLevel : 1;
            float calculatedInterval = baseSpawnInterval - ((spawnLevel - 1) * intervalReductionPerLevel);
            return Mathf.Max(calculatedInterval, minAllowedInterval);
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            CharacterController cc = Object.FindFirstObjectByType<CharacterController>();
            if (cc != null)
            {
                playerTransform = cc.transform;
                if (astronautMovement == null)
                {
                    astronautMovement = cc.GetComponent<AstronautMovement>();
                }
            }
        }

        if (upgradeMenu == null)
        {
            upgradeMenu = Object.FindFirstObjectByType<UpgradeMenuController>();
        }
    }

    void Update()
    {
        if (astronautMovement == null || !astronautMovement.isZeroGravity)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= CurrentSpawnInterval)
        {
            timer = 0f;
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        if (playerTransform == null) yield break;

        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
        Vector3 spawnPosition = playerTransform.position + randomDirection * randomDistance;

        int luckLevel = (GameManager.Instance != null) ? GameManager.Instance.luckLevel : 1;
        float bonus = (luckLevel - 1) * luckBonusMultiplier;

        float currentValuableChance = Mathf.Min(baseValuableChance + bonus, maxValuableChanceLimit);
        float currentRareChance = Mathf.Min(baseRareChance + (bonus * 0.5f), maxRareChanceLimit);
        float currentNormalChance = Mathf.Max(100f - currentValuableChance - currentRareChance, minNormalChanceLimit);

        float totalWeight = currentNormalChance + currentValuableChance + currentRareChance;
        float roll = Random.Range(0f, totalWeight);

        GameObject prefabToSpawn = null;
        float targetMinSize = 1f, targetMaxSize = 1.5f;

        if (roll < currentNormalChance)
        {
            prefabToSpawn = normalAsteroidPrefab;
            targetMinSize = normalMinSize;
            targetMaxSize = normalMaxSize;
        }
        else if (roll < currentNormalChance + currentValuableChance)
        {
            prefabToSpawn = valuableAsteroidPrefab;
            targetMinSize = valuableMinSize;
            targetMaxSize = valuableMaxSize;
        }
        else
        {
            if (rareAnimalPrefabs != null && rareAnimalPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, rareAnimalPrefabs.Length);
                prefabToSpawn = rareAnimalPrefabs[randomIndex];
                targetMinSize = animalMinSize;
                targetMaxSize = animalMaxSize;
            }
        }

        if (prefabToSpawn != null)
        {
            Collider[] colliders = prefabToSpawn.GetComponentsInChildren<Collider>();
            foreach (var col in colliders) col.enabled = false;

            GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPosition, Random.rotation);

            Collider[] cloneColliders = spawnedObj.GetComponentsInChildren<Collider>();
            foreach (var col in cloneColliders) col.enabled = false;

            Renderer rend = spawnedObj.GetComponentInChildren<Renderer>();
            float targetSize = Random.Range(targetMinSize, targetMaxSize);

            if (rend != null)
            {
                float currentSize = rend.bounds.size.magnitude;
                if (currentSize > 0.01f)
                {
                    float scaleFactor = targetSize / currentSize;
                    spawnedObj.transform.localScale = Vector3.one * scaleFactor;
                }
            }
            else
            {
                spawnedObj.transform.localScale = Vector3.one * targetSize;
            }

            yield return null;

            if (spawnedObj != null)
            {
                foreach (var col in cloneColliders) col.enabled = true;
            }
        }
    }
}