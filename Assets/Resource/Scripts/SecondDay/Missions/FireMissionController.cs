using UnityEngine;

public class FireMissionController : MonoBehaviour
{
    public GameObject fireEffect;
    public Collider2D waterZone;
    public Collider2D fireZone;

    public GameObject additionalFirePrefab;
    public Transform[] fireSpawnPoints;

    public float missionDuration = 60f;
    public int requiredExtinguishes = 10;

    public FireMissionUI fireUI;

    private float timer;
    private int extinguishCount = 0;
    private bool hasWater = false;
    private bool missionStarted = false;
    private bool missionFailed = false;

    void Update()
    {
        // ⏱ Обновляем таймер в UI
        if (fireUI != null)
            fireUI.SetTime(timer);

        // ⛔ Не продолжаем, если миссия не запущена или провалена
        if (!missionStarted || missionFailed)
            return;

        // ⌛ Уменьшаем время
        timer -= Time.deltaTime;

        // ⏳ Проверяем таймер на завершение
        if (timer <= 0f)
        {
            if (extinguishCount < requiredExtinguishes)
                FailMission();
            else
                CompleteMission();
        }

        // 📍 Получаем позицию игрока
        Vector2 playerPos = FindObjectOfType<PlayerMovement>().transform.position;

        // 💬 Показываем подсказки
        if (waterZone.OverlapPoint(playerPos) && !hasWater)
            InteractionHintController.Instance?.ShowHint(true);
        else if (fireZone.OverlapPoint(playerPos) && hasWater)
            InteractionHintController.Instance?.ShowHint(true);
        else
            InteractionHintController.Instance?.ShowHint(false);

        // 🎮 Обработка нажатия E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 💧 Взять воду
            if (waterZone.OverlapPoint(playerPos) && !hasWater)
            {
                hasWater = true;
                Debug.Log("💧 Вода набрана");
            }
            // 🔥 Потушить огонь
            else if (fireZone.OverlapPoint(playerPos) && hasWater)
            {
                hasWater = false;
                extinguishCount++;
                Debug.Log($"🔥 Потушено {extinguishCount}/{requiredExtinguishes}");

                if (fireUI != null)
                    fireUI.SetProgress(extinguishCount, requiredExtinguishes);

                if (extinguishCount >= requiredExtinguishes)
                    CompleteMission();
            }
        }
    }


    public void StartMission()
    {
        missionStarted = true;
        missionFailed = false;
        timer = missionDuration;
        extinguishCount = 0;
        hasWater = false;

        fireEffect.SetActive(true);

        if (fireUI != null)
        {
            fireUI.SetVisible(true);
            fireUI.SetProgress(0, requiredExtinguishes);
            fireUI.SetTime(missionDuration);
        }
    }

    private void CompleteMission()
    {
        missionStarted = false;
        fireEffect.SetActive(false);
        InteractionHintController.Instance?.ShowHint(false);

        Debug.Log("✅ Пожар потушен!");

        if (fireUI != null)
        {
            fireUI.SetProgress(extinguishCount, requiredExtinguishes);
            fireUI.SetVisible(false);
        }
    }

    private void FailMission()
    {
        missionStarted = false;
        missionFailed = true;

        // 🔕 Отключаем подсказку
        InteractionHintController.Instance?.ShowHint(false);

        // ❌ Скрываем UI прогресса
        if (fireUI != null)
            fireUI.SetVisible(false);

        Debug.Log("❌ Миссия провалена. Пожар вышел из-под контроля");

        // 🔥 Расширение зоны огня и блокировка прохода
        var box = fireZone as BoxCollider2D;
        if (box != null)
        {
            box.size *= 2f;                      // Увеличиваем зону
            box.offset = Vector2.zero;
            box.isTrigger = false;               // Делаем коллайдер твёрдым

            // 🔥 Спавн огня по всей площади коллайдера
            Vector2 size = box.size;
            Vector2 center = box.bounds.center;
            float step = 2f;                     // Шаг между точками спавна
            int maxFires = 200;                  // Лимит огней
            int count = 0;

            for (float x = -size.x / 2f; x <= size.x / 2f; x += step)
            {
                for (float y = -size.y / 2f; y <= size.y / 2f; y += step)
                {
                    Vector2 spawnPos = center + new Vector2(x, y);
                    Instantiate(additionalFirePrefab, spawnPos, Quaternion.identity);

                    count++;
                    if (count >= maxFires)
                        break;
                }
                if (count >= maxFires)
                    break;
            }

            Debug.Log($"🔥 Заспавнено {count} очагов огня по зоне {box.size}");
        }

        // ✅ Также можно запустить экранный эффект, тряску камеры и т.п.
    }


}
