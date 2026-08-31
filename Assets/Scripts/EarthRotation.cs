using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    void FixedUpdate()
    {
        myTransform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
    }
}