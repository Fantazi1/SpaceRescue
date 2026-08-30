using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    [Header("Зв'язок з рухом")]
    public AstronautMovement astronautMovement;

    [Header("Посилання на кістки")]
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;

    [Header("Налаштування ходьби (Walk)")]
    public float walkAnimSpeed = 15f;
    public float armSwingAngle = 40f;
    public float legSwingAngle = 45f;

    [Header("Налаштування спокою (Idle)")]
    public float idleSpeed = 2f;
    public float idleArmAngle = 5f;

    [Header("Осі обертання")]
    public Vector3 armSwingAxis = new Vector3(1f, 0, 0);
    public Vector3 legSwingAxis = new Vector3(1f, 0, 0);

    private Quaternion startLeftArm;
    private Quaternion startRightArm;
    private Quaternion startLeftLeg;
    private Quaternion startRightLeg;

    private float animTime = 0f;
    private Vector3 lastPosition;
    private bool previousZeroGState = false;

    void Start()
    {
        if (astronautMovement == null)
        {
            astronautMovement = GetComponent<AstronautMovement>();
        }

        if (leftArm) startLeftArm = leftArm.localRotation;
        if (rightArm) startRightArm = rightArm.localRotation;
        if (leftLeg) startLeftLeg = leftLeg.localRotation;
        if (rightLeg) startRightLeg = rightLeg.localRotation;

        lastPosition = transform.position;
        if (astronautMovement != null)
        {
            previousZeroGState = astronautMovement.isZeroGravity;
        }
    }

    void LateUpdate()
    {
        bool isZeroG = astronautMovement != null && astronautMovement.isZeroGravity;

        if (previousZeroGState != isZeroG)
        {
            animTime = 0f;
            previousZeroGState = isZeroG;
        }

        Vector3 currentHorizontalPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastHorizontalPos = new Vector3(lastPosition.x, 0, lastPosition.z);

        float currentSpeed = Vector3.Distance(currentHorizontalPos, lastHorizontalPos) / Time.deltaTime;
        lastPosition = transform.position;

        if (isZeroG)
        {
            float lerpSpeed = Time.deltaTime * 5f;
            if (leftLeg) leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, startLeftLeg, lerpSpeed);
            if (rightLeg) rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, startRightLeg, lerpSpeed);
            if (leftArm) leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, startLeftArm, lerpSpeed);
            if (rightArm) rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, startRightArm, lerpSpeed);
        }
        else if (currentSpeed > 0.1f)
        {
            animTime += Time.deltaTime * walkAnimSpeed * (currentSpeed / 5f);

            float armSwing = Mathf.Sin(animTime) * armSwingAngle;
            float legSwing = Mathf.Sin(animTime) * legSwingAngle;

            if (leftLeg) leftLeg.localRotation = startLeftLeg * Quaternion.Euler(legSwingAxis * legSwing);
            if (rightLeg) rightLeg.localRotation = startRightLeg * Quaternion.Euler(legSwingAxis * -legSwing);

            if (leftArm) leftArm.localRotation = startLeftArm * Quaternion.Euler(armSwingAxis * -armSwing);
            if (rightArm) rightArm.localRotation = startRightArm * Quaternion.Euler(armSwingAxis * armSwing);
        }
        else
        {
            animTime += Time.deltaTime * idleSpeed;
            float idleSway = Mathf.Sin(animTime) * idleArmAngle;

            if (leftLeg) leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, startLeftLeg, Time.deltaTime * 5f);
            if (rightLeg) rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, startRightLeg, Time.deltaTime * 5f);

            if (leftArm) leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, startLeftArm * Quaternion.Euler(0, 0, idleSway), Time.deltaTime * 5f);
            if (rightArm) rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, startRightArm * Quaternion.Euler(0, 0, -idleSway), Time.deltaTime * 5f);
        }
    }
}