using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [Header("Orbit Settings (Орбіта)")]
    public Transform earthTarget;
    public float orbitSpeed = 2.0f;

    [Header("Rotation Settings (Власна вісь)")]
    public float rotationSpeed = 5.0f;

    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    void FixedUpdate()
    {
        float fixedDelta = Time.fixedDeltaTime;

        myTransform.Rotate(Vector3.up * rotationSpeed * fixedDelta);

        if (earthTarget != null)
        {
            myTransform.RotateAround(earthTarget.position, Vector3.up, orbitSpeed * fixedDelta);
        }
    }
}