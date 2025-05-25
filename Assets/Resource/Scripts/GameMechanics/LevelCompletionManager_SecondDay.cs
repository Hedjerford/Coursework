using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelCompletionManager_SecondDay : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Button buttonFinish;
    public Button buttonContinue;

    public GameObject starsPanel;
    public Image[] starImages;
    public Sprite fullStar;
    public Sprite emptyStar;

    [Header("Статистика")]
    public FireMissionController fireMission;
    public AnimalRescueManager animalRescue;
    public TrashCounter trashCounter;

    private bool fireEnded = false;
    private bool animalsRescued = false;
    private bool trashCollected = false;

    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);
        starsPanel.SetActive(false);

        buttonFinish.onClick.AddListener(ShowStars);
        buttonContinue.onClick.AddListener(() => panel.SetActive(false));
    }

    // Метод вызывается из FireMissionController
    public void OnFireTimerEnd()
    {
        fireEnded = true;
        CheckCompletion();
    }

    // Вызывается из AnimalRescueManager
    public void CheckCompletion()
    {
        if (shown) return;

        bool fireComplete = fireEnded;
        bool animalComplete = animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals;
        bool trashComplete = trashCounter != null && trashCounter.collectedCount >= 3;

        animalsRescued = animalComplete;
        trashCollected = trashComplete;

        bool allCompleted = fireComplete && animalComplete && trashComplete;
        bool anyCompleted = fireComplete || animalComplete || trashComplete;

        shown = true;

        if (allCompleted)
        {
            // ✅ Все миссии выполнены — сразу показываем звёзды
            ShowStars();
        }
        else if (anyCompleted)
        {
            // ⚠️ Частично выполнено — показываем обычную панель
            panel.SetActive(true);
        }
    }

    public void ShowStars()
    {
        FindObjectOfType<GameTimer>()?.StopTimer();

        // ❗ Показываем сразу панель со звездами
        panel.SetActive(false);
        starsPanel.SetActive(true);

        int starCount = 0;

        if (FireMissionController.SuccessMission)
            starCount++;

        if (animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals) starCount++;
        if (trashCounter != null && trashCounter.collectedCount >= 5) starCount++;

        for (int i = 0; i < starImages.Length; i++)
            starImages[i].sprite = i < starCount ? fullStar : emptyStar;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        if (fireEnded && animalsRescued && trashCollected)
        {
            PlayerPrefs.SetInt("HQDialogueStage", 4);
        }
        else
        {
            PlayerPrefs.SetInt("HQDialogueStage", 5);
        }

        SceneManager.LoadScene("Headquarters_Cutscene");
    }
}
