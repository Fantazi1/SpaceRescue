using UnityEngine;
using UnityEngine.UI; 

public class SpaceBounds : MonoBehaviour
{
    [Header("Центр та налаштування")]
    public Transform centerPoint;
    public float maxDistance = 50f;
    public float slowDownZone = 5f;

    [Header("Ефект меж (Віньєтка)")]
    public Image vignetteImage;          
    public Color vignetteColor = new Color(0f, 0.5f, 1f); 
    public float maxVignetteAlpha = 0.8f;

    [Header("Посилання")]
    public AstronautMovement astronautMovement;

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
    }

    void Update()
    {
        if (astronautMovement == null || !astronautMovement.isZeroGravity) return;
        if (centerPoint == null) return;

        Vector3 direction = transform.position - centerPoint.position;
        float distance = direction.magnitude;

        UpdateVignette(distance);

        if (distance >= maxDistance - slowDownZone)
        {
            Vector3 outwardVelocity = Vector3.Project(astronautMovement.velocity, direction.normalized);
            if (Vector3.Dot(outwardVelocity, direction.normalized) > 0)
            {
                astronautMovement.velocity -= outwardVelocity;
            }

            if (distance > maxDistance)
            {
                transform.position = centerPoint.position + direction.normalized * maxDistance;
            }
        }
    }

    private void UpdateVignette(float distance)
    {
        if (vignetteImage == null) return;

        float startZone = maxDistance - slowDownZone;

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
}