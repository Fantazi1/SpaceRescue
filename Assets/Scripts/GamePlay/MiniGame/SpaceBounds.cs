using UnityEngine;

public class SpaceBounds : MonoBehaviour
{
    [Header("Центр та налаштування")]
    public Transform centerPoint;
    public float maxDistance = 50f;
    public float slowDownZone = 5f; 

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
    }

    void Update()
    {
        if (astronautMovement == null || !astronautMovement.isZeroGravity) return;
        if (centerPoint == null) return;

        Vector3 direction = transform.position - centerPoint.position;
        float distance = direction.magnitude;

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
}