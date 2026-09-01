using UnityEngine;
using UnityEngine.UI;

public class SpaceBounds : MonoBehaviour
{
    [Header("Центр та налаштування")]
    public Transform centerPoint;
    public float baseMaxDistance = 50f;
    public float upgradeDistanceStep = 20f;
    public float slowDownZone = 5f;

    [Header("Візуальна куля")]
    [Tooltip("Увімкнути/вимкнути відображення візуальної сфери межі")]
    public bool isVisualEnabled = true;
    public Color sphereColor = new Color(0f, 0.6f, 1f, 0.08f);

    [Header("Ефект меж (Віньєтка)")]
    public Image vignetteImage;
    public Color vignetteColor = new Color(0f, 0.5f, 1f);
    public float maxVignetteAlpha = 0.8f;

    [Header("Посилання")]
    public AstronautMovement astronautMovement;

    private GameObject boundarySphere;
    private Material sphereMaterial;
    private int lastZoneLevel = -1;
    private bool lastVisualState = true;

    private float CurrentMaxDistance
    {
        get
        {
            if (GameManager.Instance != null)
            {
                return baseMaxDistance + ((GameManager.Instance.zoneLevel - 1) * upgradeDistanceStep);
            }
            return baseMaxDistance;
        }
    }

    void Start()
    {
        if (astronautMovement == null)
        {
            astronautMovement = GetComponent<AstronautMovement>();
        }

        if (centerPoint == null)
        {
            ShipReturnTerminal shipTerminal = Object.FindFirstObjectByType<ShipReturnTerminal>();
            if (shipTerminal != null)
            {
                centerPoint = shipTerminal.transform;
            }
        }

        if (vignetteImage != null)
        {
            Color c = vignetteColor;
            c.a = 0f;
            vignetteImage.color = c;
        }

        CreateBoundarySphere();
    }

    void Update()
    {
        if (boundarySphere != null && lastVisualState != isVisualEnabled)
        {
            lastVisualState = isVisualEnabled;
            boundarySphere.SetActive(isVisualEnabled);
        }

        if (centerPoint != null && boundarySphere != null)
        {
            boundarySphere.transform.position = centerPoint.position;

            int currentLevel = GameManager.Instance != null ? GameManager.Instance.zoneLevel : 1;
            if (currentLevel != lastZoneLevel)
            {
                lastZoneLevel = currentLevel;
                float diameter = CurrentMaxDistance * 2f;
                boundarySphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            }
        }
    }

    void LateUpdate()
    {
        if (astronautMovement == null || !astronautMovement.isZeroGravity) return;
        if (centerPoint == null) return;

        Vector3 centerPos = centerPoint.position;
        Vector3 direction = transform.position - centerPos;
        float distance = direction.magnitude;
        float maxDist = CurrentMaxDistance;

        UpdateVignette(distance);

        if (distance >= maxDist - slowDownZone)
        {
            Vector3 outwardVelocity = Vector3.Project(astronautMovement.velocity, direction.normalized);
            if (Vector3.Dot(outwardVelocity, direction.normalized) > 0)
            {
                astronautMovement.velocity -= outwardVelocity;
            }

            if (distance >= maxDist)
            {
                transform.position = centerPos + direction.normalized * maxDist;

                astronautMovement.velocity = Vector3.ProjectOnPlane(astronautMovement.velocity, direction.normalized);
            }
        }
    }

    private void CreateBoundarySphere()
    {
        boundarySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boundarySphere.name = "VisualSpaceBoundarySphere";

        Collider col = boundarySphere.GetComponent<Collider>();
        if (col != null) Destroy(col);

        Shader shader = Shader.Find("Sprites/Default");
        sphereMaterial = new Material(shader);

        Color finalColor = new Color(sphereColor.r, sphereColor.g, sphereColor.b, 0.08f);
        sphereMaterial.color = finalColor;

        MeshRenderer renderer = boundarySphere.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = sphereMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        float diameter = CurrentMaxDistance * 2f;
        boundarySphere.transform.localScale = new Vector3(diameter, diameter, diameter);

        boundarySphere.SetActive(isVisualEnabled);
        lastVisualState = isVisualEnabled;
    }

    private void UpdateVignette(float distance)
    {
        if (vignetteImage == null) return;

        float startZone = CurrentMaxDistance - slowDownZone;

        if (distance >= startZone)
        {
            float t = Mathf.Clamp01((distance - startZone) / slowDownZone);

            Color c = vignetteColor;
            c.a = Mathf.Lerp(0f, maxVignetteAlpha, t);
            vignetteImage.color = c;
        }
        else
        {
            if (vignetteImage.color.a > 0f)
            {
                Color c = vignetteColor;
                c.a = 0f;
                vignetteImage.color = c;
            }
        }
    }

    void OnDestroy()
    {
        if (sphereMaterial != null)
        {
            Destroy(sphereMaterial);
        }
    }
}