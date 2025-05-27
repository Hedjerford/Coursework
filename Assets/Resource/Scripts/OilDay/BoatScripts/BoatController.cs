using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f;

    [Header("Ограничения движения по воде")]
    public Vector2 minBounds; // Левый нижний угол зоны воды
    public Vector2 maxBounds; // Правый верхний угол зоны воды

    private bool isOnWater = false;

    void Update()
    {
        // Всегда принимаем управление, но двигаем лодку только если она на воде
        float move = Input.GetAxis("Vertical");
        float rotate = -Input.GetAxis("Horizontal");

        if (isOnWater)
        {
            transform.Translate(Vector3.up * move * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward * rotate * turnSpeed * Time.deltaTime);
        }

        ClampPosition(); // ⛔ Ограничение по границам воды
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
