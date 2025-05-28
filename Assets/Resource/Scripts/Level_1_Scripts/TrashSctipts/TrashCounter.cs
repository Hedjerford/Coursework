using UnityEngine;
using TMPro;

public class TrashCounter : MonoBehaviour
{
    public TextMeshProUGUI trashText;
    public int totalTrash = 8;

    public int collectedCount = 0;

    private void Start()
    {
        UpdateUI();
    }

    public void CollectTrash()
    {
        collectedCount++;
        UpdateUI();
        if (collectedCount >= 10)
        {
            AchievementManager.Instance.Unlock("Сборщик");
        }
        if (collectedCount >= 1)
        {
            FindObjectOfType<LevelCompletionManager_SecondDay>()?.CheckCompletion();
            FindObjectOfType<LevelCompletionManager>()?.MarkTrashCollected();
        }

    }

    private void UpdateUI()
    {
        trashText.text = $"Мусора собрано {collectedCount} / {totalTrash}";
    }
}
