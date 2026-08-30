using System.Collections;
using UnityEngine;

public class SpaceSpawnManager : MonoBehaviour
{
    [Header("Префаби")]
    public GameObject normalAsteroidPrefab;
    public GameObject valuableAsteroidPrefab;

    [Header("Масив рідкісних тваринок (можна додати скільки завгодно)")]
    public GameObject[] rareAnimalPrefabs;

    [Header("Налаштування спавну")]
    public Transform playerTransform;
    public AstronautMovement astronautMovement;
    public float spawnRadius = 30f;
    public float minSpawnDistance = 12f;
    public float spawnInterval = 2f;

    [Header("Шанси спавну (у відсотках)")]
    [Range(0f, 100f)] public float normalAsteroidChance = 60f;
    [Range(0f, 100f)] public float valuableAsteroidChance = 30f;
    [Range(0f, 100f)] public float rareAnimalChance = 10f;

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
    }

    void Update()
    {
        if (astronautMovement == null || !astronautMovement.isZeroGravity)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
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

        float totalWeight = normalAsteroidChance + valuableAsteroidChance + rareAnimalChance;
        float roll = Random.Range(0f, totalWeight);

        GameObject prefabToSpawn = null;
        float targetMinSize = 1f, targetMaxSize = 1.5f;

        if (roll < normalAsteroidChance)
        {
            prefabToSpawn = normalAsteroidPrefab;
            targetMinSize = normalMinSize;
            targetMaxSize = normalMaxSize;
        }
        else if (roll < normalAsteroidChance + valuableAsteroidChance)
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