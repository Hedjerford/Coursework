using UnityEngine;
using TMPro;

public class BoomMissionUI : MonoBehaviour
{
    public static BoomMissionUI Instance;

    public TextMeshProUGUI boomCounterText;
    public TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool timerRunning = false;
    private int total;
    private int placed;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMissionUI(int totalBooms, float duration)
    {
        total = totalBooms;
        placed = 0;
        timeRemaining = duration;
        timerRunning = true;

        boomCounterText.text = $"Установите боны: 0 / {total}";
        boomCounterText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
    }

    public void UpdateBoomCount(int count)
    {
        placed = count;
        boomCounterText.text = $"Установите боны: {placed} / {total}";
    }

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerRunning = false;
            timerText.text = "Время вышло!";
            BoomPlacementController.Instance?.OnTimerEnd();
        }
        else
        {
            timerText.text = $"Осталось: {Mathf.CeilToInt(timeRemaining)}";
        }
    }

    public void Hide()
    {
        boomCounterText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }
}
