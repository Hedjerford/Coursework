using UnityEngine;
using TMPro;

public class TreeCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public int plantedTrees = 0;
    public int targetTrees = 5;

    public void AddTree()
    {
        plantedTrees++;
        plantedTrees = Mathf.Min(plantedTrees, targetTrees); // чтобы не превысить максимум
        UpdateUI();
        if (plantedTrees == 10)
        {
            AchievementManager.Instance.Unlock("Спасатель O₂");
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        counterText.text = $"Деревьев посажено: {plantedTrees} / {targetTrees}";
    }
}
