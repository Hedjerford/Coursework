using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelCompletionManager : MonoBehaviour
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
    public TreeCounter treeCounter;
    public BirdCounter birdCounter;
    public TrashCounter trashCounter;

    private bool treePlanted;
    private bool birdFed;
    private bool trashCollected;

    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);
        starsPanel.SetActive(false);

        buttonFinish.onClick.AddListener(ShowStars);
        buttonContinue.onClick.AddListener(() => panel.SetActive(false));
    }

    public void MarkTreePlanted()
    {
        treePlanted = true;
        CheckCompletion();
    }

    public void MarkBirdFed()
    {
        birdFed = true;
        CheckCompletion();
    }

    public void MarkTrashCollected()
    {
        trashCollected = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (!shown && treePlanted && birdFed && trashCollected)
        {
            shown = true;
            panel.SetActive(true);
        }
    }

    private void ShowStars()
    {
        FindObjectOfType<GameTimer>().StopTimer();

        panel.SetActive(false);
        starsPanel.SetActive(true);

        int starCount = 0;

        if (treeCounter.plantedTrees >= 5) starCount++;
        if (birdCounter.fedBirds >= 3) starCount++;
        if (trashCounter.collectedCount >= 8) starCount++;

        for (int i = 0; i < starImages.Length; i++)
            starImages[i].sprite = i < starCount ? fullStar : emptyStar;

        // Можно добавить кнопку перехода на следующий уровень
        // или автопереход через 5 секунд
    }

    public void ShowCompletionResult()
{
    starsPanel.SetActive(true);

    int starCount = 0;

    if (treeCounter.plantedTrees >= 5) starCount++;
    if (birdCounter.fedBirds >= 3) starCount++;
    if (trashCounter.collectedCount >= 8) starCount++;

    for (int i = 0; i < starImages.Length; i++)
        starImages[i].sprite = i < starCount ? fullStar : emptyStar;

}
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        if (treeCounter.plantedTrees >= 5 && birdCounter.fedBirds >= 3 && trashCounter.collectedCount >= 8)
        {
            PlayerPrefs.SetInt("HQDialogueStage", 3); // диалог 3
        }
        else
        {
            PlayerPrefs.SetInt("HQDialogueStage", 2); // диалог 2
        }

        //PlayerPrefs.Save(); // обязательно перед загрузкой сцены
        SceneManager.LoadScene("Headquarters_Cutscene"); // подставь нужное имя
    }
}
