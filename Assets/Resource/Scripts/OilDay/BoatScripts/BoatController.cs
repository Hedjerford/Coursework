using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f;

    private bool isOnWater = false;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        // Автоматически найти объект с тегом Water и взять границы коллайдера
        GameObject waterObject = GameObject.FindGameObjectWithTag("Water");

        if (waterObject != null)
        {
            Collider2D waterCollider = waterObject.GetComponent<Collider2D>();

            if (waterCollider != null)
            {
                Bounds bounds = waterCollider.bounds;
                minBounds = bounds.min;
                maxBounds = bounds.max;
                Debug.Log($"🌊 Границы воды определены: min={minBounds}, max={maxBounds}");
            }
            else
            {
                Debug.LogWarning("❗ Объект с тегом 'Water' не содержит Collider2D");
            }
        }
        else
        {
            Debug.LogWarning("❗ Не найден объект с тегом 'Water'");
        }
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        float rotate = -Input.GetAxis("Horizontal");

        if (isOnWater)
        {
            transform.Translate(Vector3.up * move * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward * rotate * turnSpeed * Time.deltaTime);
        }

        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isOnWater = true;
            Debug.Log("🟦 Катер вошёл в воду");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isOnWater = false;
            Debug.Log("⬜ Катер вышел из воды");
        }
    }
}
