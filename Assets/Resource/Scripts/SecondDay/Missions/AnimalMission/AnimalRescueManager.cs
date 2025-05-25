using UnityEngine;
using TMPro;

public class AnimalRescueManager : MonoBehaviour
{
    public static AnimalRescueManager Instance;

    public int totalAnimals = 5;
    public int rescuedAnimals = 0;

    [Header("UI")]
    public TextMeshProUGUI rescueCounterText; // 👈 Подключи в инспекторе

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateUI();
    }

    public void RegisterRescue()
    {
        rescuedAnimals++;
        Debug.Log($"✅ Спасено: {rescuedAnimals}/{totalAnimals}");

        UpdateUI();

        if (rescuedAnimals >= totalAnimals)
        {
            Debug.Log("🎉 Все звери спасены!");
            // Тут можно вызвать победу
        }
        if (rescuedAnimals >= totalAnimals)
        {
            FindObjectOfType<LevelCompletionManager_SecondDay>()?.CheckCompletion();
        }


    }

    private void UpdateUI()
    {
        if (rescueCounterText != null)
            rescueCounterText.text = $"Зверей спасено: {rescuedAnimals} / {totalAnimals}";
    }
}
