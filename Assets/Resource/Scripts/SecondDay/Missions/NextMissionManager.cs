using UnityEngine;
using UnityEngine.SceneManagement;

public class NextMissionManager : MonoBehaviour
{
    public static NextMissionManager Instance;

    [Header("Список миссий (по порядку)")]
    public string[] missionSceneNames;

    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNext()
    {
        currentIndex++;

        if (currentIndex < missionSceneNames.Length)
        {
            string nextScene = missionSceneNames[currentIndex];
            Debug.Log($"▶ Загружаем следующую миссию: {nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("🎉 Все миссии завершены! Можно показать экран победы или титры.");
            // Можно вызвать финальную сцену или UI
        }
    }

    public void RestartCurrent()
    {
        if (currentIndex >= 0 && currentIndex < missionSceneNames.Length)
        {
            SceneManager.LoadScene(missionSceneNames[currentIndex]);
        }
    }

    public void ResetMissions()
    {
        currentIndex = -1;
    }
}
