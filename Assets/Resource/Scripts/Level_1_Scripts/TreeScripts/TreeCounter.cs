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
        plantedTrees = Mathf.Min(plantedTrees, targetTrees); // ����� �� ��������� ��������
        UpdateUI();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        counterText.text = $"�������� ��������: {plantedTrees} / {targetTrees}";
    }
}
