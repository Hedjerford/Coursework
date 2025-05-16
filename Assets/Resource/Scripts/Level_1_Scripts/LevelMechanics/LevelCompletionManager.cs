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
        // Здесь можешь указать конкретную сцену по имени или по индексу
        SceneManager.LoadScene("NextLevelScene"); // Замени на нужное имя
    }
}
