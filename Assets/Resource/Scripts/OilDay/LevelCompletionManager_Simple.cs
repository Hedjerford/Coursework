using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompletionManager_Simple : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Button buttonFinish;

    private bool shown = false;

    private void Start()
    {
        panel.SetActive(false);

        buttonFinish.onClick.AddListener(Next);
    }

    // Вызывается, когда круг из бонов построен
    public void ShowCompletionPanel()
    {
        if (shown) return;

        shown = true;
        panel.SetActive(true);
    }

    public void Next()
    {
        if (!BoomPlacementController.TimerEnd)
        {
            PlayerPrefs.SetInt("HQDialogueStage", 6); 
        }
        else
        {
            PlayerPrefs.SetInt("HQDialogueStage", 7); 
        }

        SceneManager.LoadScene("Headquarters_Cutscene");
    }
}
