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
    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);
        starsPanel.SetActive(false);

        buttonFinish.onClick.AddListener(ShowStars);
        buttonContinue.onClick.AddListener(() => panel.SetActive(false));
    }

    public void OnFireTimerEnd()
    {
        fireEnded = true;
    }

    // Новый единый метод проверки
    private bool IsAllObjectivesCompleted()
    {
        bool fireComplete = fireEnded;
        bool animalComplete = animalRescue != null && animalRescue.rescuedAnimals >= 1;
        bool trashComplete = trashCounter != null && trashCounter.collectedCount >= 1;

        return fireComplete && animalComplete && trashComplete;
    }

    // Также можно сделать метод частичного выполнения
    private bool IsAnyObjectiveCompleted()
    {
        bool fireComplete = fireEnded;
        bool animalComplete = animalRescue != null && animalRescue.rescuedAnimals >= 1;
        bool trashComplete = trashCounter != null && trashCounter.collectedCount >= 1;

        return fireComplete || animalComplete || trashComplete;
    }

    public void CheckCompletion()
    {
        if (shown) return;

        shown = true;

        if (IsAllObjectivesCompleted())
        {
            ShowStars(); // все выполнено
        }
        else if (IsAnyObjectiveCompleted())
        {
            panel.SetActive(true); // частично выполнено
        }
    }

    public void ShowStars()
    {
        FindObjectOfType<GameTimer>()?.StopTimer();

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
        if (IsAllObjectivesCompleted())
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
