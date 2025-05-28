using UnityEngine;
using TMPro;

public class BirdCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public int fedBirds = 0;
    public int targetBirds = 3;
    private int AchivementBird = 5;

    public void FeedBird()
    {
        fedBirds++;
        fedBirds = Mathf.Min(fedBirds, AchivementBird);
        UpdateUI();
        if (fedBirds == 5)
        {
            AchievementManager.Instance.Unlock("Бренда Фрикер");
        }
        FindObjectOfType<LevelCompletionManager>().MarkBirdFed();

    }

    private void Start()
    {
        UpdateUI();
        AchievementManager.Instance.Unlock("Начало...");
    }

    private void UpdateUI()
    {
        counterText.text = $"Птичек накормлено: {fedBirds} / {targetBirds}";
    }
}
