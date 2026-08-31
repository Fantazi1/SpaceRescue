using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Налаштування руху")]
    public float moveSpeed = 1.5f;
    public float fadeSpeed = 1.5f;
    public Vector3 offset = new Vector3(0, 1f, 0);

    private TextMeshPro textMesh;
    private Color textColor;

    public void Setup(string text, Color color)
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
            textColor = color;
        }

        transform.position += offset;
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}