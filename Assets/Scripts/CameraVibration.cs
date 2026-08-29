using UnityEngine;

public class CameraVibration : MonoBehaviour
{
    public float shakeIntensity = 0.05f;
    public float shakeSpeed = 20f;      

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        float noiseX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;

        transform.localPosition = originalPosition + new Vector3(noiseX, noiseY, 0f) * shakeIntensity;
    }
}