using UnityEngine;

public class SpaceCollectible : MonoBehaviour
{
    public enum ObjectType { NormalAsteroid, ValuableAsteroid, RareAnimal }
    public ObjectType objectType;

    [Header("Рух")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    private float moveSpeed;
    private Vector3 moveDirection;

    void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        moveDirection = Random.onUnitSphere;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Destroy(gameObject, 25f); 
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * 15f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            HandleCollection();
        }
    }

    public void HandleCollection()
    {
        if (objectType == ObjectType.ValuableAsteroid)
        {
            Debug.Log("Підібрано цінний астероїд! +Валюта");
        }
        else if (objectType == ObjectType.RareAnimal)
        {
            Debug.Log("Спіймано рідкісну космічну тваринку!");
        }
        else
        {
            return;
        }

        Destroy(gameObject);
    }
}