using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [Header("Orbit Settings (Орбіта)")]
    public Transform earthTarget;     
    public float orbitSpeed = 2.0f; 

    [Header("Rotation Settings (Власна вісь)")]
    public float rotationSpeed = 5.0f; 

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (earthTarget != null)
        {
            transform.RotateAround(earthTarget.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }
}
