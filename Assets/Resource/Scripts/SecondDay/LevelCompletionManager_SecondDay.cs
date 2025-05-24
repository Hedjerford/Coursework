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
    public TrashCounter trashCounter;
    public AnimalRescueManager animalRescue;
    public FireMissionController fireMission; // ✅ можно передавать статус пожара

    private bool trashCollected;
    private bool animalsRescued;
    private bool fireExtinguished;

    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);
        starsPanel.SetActive(false);

        buttonFinish.onClick.AddListener(ShowStars);
        buttonContinue.onClick.AddListener(() => panel.SetActive(false));
    }

    public void MarkTrashCollected()
    {
        trashCollected = true;
        CheckCompletion();
    }

    public void MarkFireExtinguished()
    {
        fireExtinguished = true;
        CheckCompletion();
    }

    public void MarkAnimalsRescued()
    {
        animalsRescued = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (shown) return;

        bool allTrash = trashCounter.collectedCount >= 8;
        bool allAnimals = animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals;
        bool fireOut = FireMissionController.SuccessMission;

        if (allTrash && allAnimals && fireOut)
        {
            shown = true;
            ShowStars(); // или panel.SetActive(true) если хочешь паузу
        }
    }


    private void ShowStars()
    {
        panel.SetActive(false);
        starsPanel.SetActive(true);

        int starCount = 0;

        if (trashCounter.collectedCount >= 8) starCount++;
        if (animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals) starCount++;
        if (fireMission != null && FireMissionController.SuccessMission) starCount++;

        for (int i = 0; i < starImages.Length; i++)
            starImages[i].sprite = i < starCount ? fullStar : emptyStar;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        int starCount = 0;

        if (trashCounter.collectedCount >= 8) starCount++;
        if (animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals) starCount++;
        if (fireMission != null && FireMissionController.SuccessMission) starCount++;

        PlayerPrefs.SetInt("HQDialogueStage", starCount == 3 ? 3 : 2);
        SceneManager.LoadScene("Headquarters_Cutscene");
    }


}
