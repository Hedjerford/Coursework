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
    public FireMissionController fireMission;

    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);
        starsPanel.SetActive(false);

        buttonFinish.onClick.AddListener(ShowStars);
        buttonContinue.onClick.AddListener(() => panel.SetActive(false));
    }

    public void CheckCompletion()
    {
        if (shown) return;

        bool allTrash = trashCounter.collectedCount >= 8;
        bool allAnimals = animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals;
        bool fireOut = FireMissionController.SuccessMission;

        int completed = 0;
        if (allTrash) completed++;
        if (allAnimals) completed++;
        if (fireOut) completed++;

        if (completed >= 1)
        {
            // ❌ Скрываем стартовую панель, если хотя бы одно выполнено
            panel.SetActive(false);
        }

        if (completed == 3)
        {
            // ✅ Всё выполнено — сразу показываем звёзды
            shown = true;
            ShowStars();
        }
    }

    public void OnFireTimerEnd()
    {
        // ⏰ Вызывается из FireMissionController при завершении таймера
        if (!shown)
        {
            panel.SetActive(true); // показать финальную панель, если звезды не показаны
        }
    }

    public void ShowStars()
    {
        panel.SetActive(false);
        starsPanel.SetActive(true);

        int starCount = 0;

        if (trashCounter.collectedCount >= 8) starCount++;
        if (animalRescue != null && animalRescue.rescuedAnimals >= animalRescue.totalAnimals) starCount++;
        if (FireMissionController.SuccessMission) starCount++;

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
        if (FireMissionController.SuccessMission) starCount++;

        PlayerPrefs.SetInt("HQDialogueStage", starCount == 3 ? 3 : 2);
        SceneManager.LoadScene("Headquarters_Cutscene");
    }
}
