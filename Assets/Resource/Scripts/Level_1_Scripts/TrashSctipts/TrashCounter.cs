using UnityEngine;
using TMPro;

public class TrashCounter : MonoBehaviour
{
    public TextMeshProUGUI trashText;
    public int totalTrash = 5;

    private int collectedCount = 0;

    private void Start()
    {
        UpdateUI();
    }

    public void CollectTrash()
    {
        collectedCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        trashText.text = $"Мусора собрано {collectedCount} / {totalTrash}";
    }
}
