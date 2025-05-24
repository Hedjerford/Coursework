using UnityEngine;
using TMPro;

public class TrashCounter : MonoBehaviour
{
    public TextMeshProUGUI trashText;
    public int totalTrash = 5;

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
            AchievementManager.Instance.Unlock("—борщик");
        }

    }

    private void UpdateUI()
    {
        trashText.text = $"ћусора собрано {collectedCount} / {totalTrash}";
    }
}
