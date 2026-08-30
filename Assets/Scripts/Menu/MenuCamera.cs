using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public Transform pos1Menu;
    public Transform pos2Settings;
    public Transform pos3Exit;

    public float moveSpeed = 3f;
    private Transform currentTarget;

    void Start()
    {
        currentTarget = pos1Menu;
        transform.position = currentTarget.position;
        transform.rotation = currentTarget.rotation;
    }

    void Update()
    {
        if (currentTarget == null) return;

        transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * moveSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, Time.deltaTime * moveSpeed);
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
}