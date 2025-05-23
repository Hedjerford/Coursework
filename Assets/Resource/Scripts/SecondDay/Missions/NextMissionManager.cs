using UnityEngine;

public class NextMissionManager : MonoBehaviour
{
    public static NextMissionManager Instance;

    [Header("Миссии по порядку")]
    public GameObject[] missions;

    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // если надо сохранить при переходе сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNext()
    {
        // Деактивируем предыдущую
        if (currentIndex >= 0 && currentIndex < missions.Length)
        {
            missions[currentIndex].SetActive(false);
        }

        currentIndex++;

        if (currentIndex < missions.Length)
        {
            Debug.Log($"▶ Активируем миссию #{currentIndex + 1}: {missions[currentIndex].name}");
            missions[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("🎉 Все миссии завершены!");
            // Тут можно включить финальный экран или титры
        }
    }

    public void RestartCurrent()
    {
        if (currentIndex >= 0 && currentIndex < missions.Length)
        {
            missions[currentIndex].SetActive(false);
            missions[currentIndex].SetActive(true);
        }
    }
}
