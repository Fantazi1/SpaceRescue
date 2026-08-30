using UnityEngine;

public class ShipHover : MonoBehaviour
{
    [Header("Hover Settings (Постійна висота)")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1.5f;
    public float rollAmount = 1.0f;   

    [Header("Random Drift (Випадковий рух носа)")]
    public float driftAmount = 2.5f;
    public float driftSpeed = 0.4f;  

    [Header("Vibration Settings")]
    public float shakeIntensity = 0.015f;
    public float shakeSpeed = 35f;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;

    private float seedX;
    private float seedY;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        float noiseX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;
        float noiseZ = Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) * 2f - 1f;

        transform.localPosition = startLocalPos + new Vector3(noiseX * shakeIntensity, yOffset + (noiseY * shakeIntensity), noiseZ * shakeIntensity);

        float driftTime = Time.time * driftSpeed;

        float pitchDrift = (Mathf.PerlinNoise(driftTime + seedX, 0f) * 2f - 1f) * driftAmount;
        float yawDrift = (Mathf.PerlinNoise(0f, driftTime + seedY) * 2f - 1f) * driftAmount;

        float roll = Mathf.Cos(Time.time * floatFrequency) * rollAmount;

        Quaternion driftRotation = Quaternion.Euler(pitchDrift, yawDrift, roll);
        transform.localRotation = startLocalRot * driftRotation;
    }
}