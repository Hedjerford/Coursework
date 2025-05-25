using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 180f; // 3 минуты
    public TextMeshProUGUI timerText;
    public CanvasGroup blackOverlay;
    public GameObject resultPanel;
    public LevelCompletionManager_SecondDay levelCompletionManager;

    private bool levelEnded = false;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0, timeRemaining);
        UpdateTimerUI();

        if (timeRemaining <= 0)
        {
            isRunning = false;
            EndLevel();
        }
       

    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void EndLevel()
    {
        levelEnded = true;
        StartCoroutine(FadeToBlackAndShowResult());
    }

    System.Collections.IEnumerator FadeToBlackAndShowResult()
    {
        float duration = 1.5f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            blackOverlay.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        blackOverlay.alpha = 1;

        if (levelCompletionManager != null)
            levelCompletionManager.ShowStars(); // ✅ правильный вызов
    }

}
