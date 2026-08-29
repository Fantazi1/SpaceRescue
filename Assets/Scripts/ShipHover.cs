using UnityEngine;

public class ShipHover : MonoBehaviour
{
    public float floatAmplitude = 0.2f; 
    public float floatFrequency = 1.5f;
    public float tiltAmount = 2.0f;    

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0, yOffset, 0);

        float tilt = Mathf.Sin(Time.time * floatFrequency) * tiltAmount;
        Quaternion tiltRotation = Quaternion.Euler(tilt, 0, tilt / 2f);

        transform.localRotation = startLocalRot * tiltRotation;
    }
}