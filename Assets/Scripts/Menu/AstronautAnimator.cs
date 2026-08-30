using UnityEngine;

public class AstronautAnimator : MonoBehaviour
{
    public Transform armBone;

    public enum Role { Mechanic, WavingGuy }
    public Role astronautRole = Role.Mechanic;

    [Header("Налаштування ремонту (Mechanic)")]
    [Tooltip("Вісь, по якій він крутить ключ (спробуйте X, Y або Z)")]
    public Vector3 wrenchTwistAxis = new Vector3(1f, 0, 0);
    public float twistAmplitude = 60f; 

    [Header("Налаштування прощання (Waving Guy)")]
    public Vector3 armRaiseAngle = new Vector3(0, 0, 80f);
    public Vector3 waveSwingAxis = new Vector3(0, 1f, 0);
    public float waveAmplitude = 60f;
    public float waveSpeed = 12f;

    private Quaternion defaultRot;
    private bool isActive = false;
    private float actionTimer = 0f;
    private float animationTime = 0f;

    void Start()
    {
        if (armBone != null) defaultRot = armBone.localRotation;
    }

    void Update()
    {
        if (armBone == null) return;

        Quaternion targetRotation = defaultRot;

        if (astronautRole == Role.Mechanic)
        {
            float idleSway = Mathf.Sin(Time.time * 1.5f) * 3f;
            Vector3 baseSway = new Vector3(idleSway, idleSway, 0);

            if (!isActive)
            {
                float cycle = Mathf.Repeat(Time.time * 0.5f, 1f);
                float angle = 0f;

                if (cycle < 0.2f)
                    angle = Mathf.Lerp(0f, twistAmplitude, cycle / 0.2f);
                else if (cycle < 0.3f)
                    angle = Mathf.Lerp(twistAmplitude, 0f, (cycle - 0.2f) / 0.1f); 

                targetRotation = defaultRot * Quaternion.Euler(baseSway + (wrenchTwistAxis * angle));
            }
            else
            {
                animationTime += Time.deltaTime;
                float cycle = Mathf.Repeat(animationTime * 3f, 1f);

                float angle = 0f;
                if (cycle < 0.6f)
                    angle = Mathf.Lerp(0f, twistAmplitude, cycle / 0.6f);
                else
                    angle = Mathf.Lerp(twistAmplitude, 0f, (cycle - 0.6f) / 0.4f);

                float effortShake = (Mathf.PerlinNoise(animationTime * 15f, 0f) - 0.5f) * 8f;
                Vector3 shakeVec = new Vector3(effortShake, effortShake, effortShake);

                targetRotation = defaultRot * Quaternion.Euler(baseSway + shakeVec + (wrenchTwistAxis * angle));

                actionTimer -= Time.deltaTime;
                if (actionTimer <= 0) isActive = false;
            }
        }
        else if (astronautRole == Role.WavingGuy)
        {
            if (isActive)
            {
                animationTime += Time.deltaTime;
                float wave = Mathf.Sin(animationTime * waveSpeed) * waveAmplitude;
                Vector3 finalAngle = armRaiseAngle + (waveSwingAxis * wave);
                targetRotation = defaultRot * Quaternion.Euler(finalAngle);
            }
        }

        armBone.localRotation = Quaternion.Lerp(armBone.localRotation, targetRotation, Time.deltaTime * 25f);
    }

    public void TriggerAction()
    {
        isActive = true;
        actionTimer = 0.8f; 
        animationTime = 0f;
    }
}