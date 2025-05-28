using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [TextArea(3, 10)] public string[] dialogueLines;
    public TextMeshProUGUI dialogueText;

    public float typingSpeed = 0.03f;

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    public GameObject continueHint;

    [Header("Диалоги")]
    [TextArea(3, 10)] public string[] firstVisitLines;
    [TextArea(3, 10)] public string[] BadDialogue;
    [TextArea(3, 10)] public string[] GoodDialogue;
    [TextArea(3, 10)] public string[] EndSecondLevelGood;
    [TextArea(3, 10)] public string[] EndSecondLevelBad;
    [TextArea(3, 10)] public string[] OilLevelGood;
    [TextArea(3, 10)] public string[] OilLevelBad;

    private void Awake()
    {
        // Только для тестов — удаление всех PlayerPrefs
        // PlayerPrefs.DeleteAll();

        if (!PlayerPrefs.HasKey("HasLaunched"))
        {
            PlayerPrefs.SetInt("HQDialogueStage", 1);
            PlayerPrefs.SetInt("HasLaunched", 1);
            PlayerPrefs.Save();
        }

        int stage = PlayerPrefs.GetInt("HQDialogueStage", 1);

        switch (stage)
        {
            case 1: dialogueLines = firstVisitLines; break;
            case 2: dialogueLines = BadDialogue; break;
            case 3: dialogueLines = GoodDialogue; break;
            case 4: dialogueLines = EndSecondLevelGood; break;
            case 5: dialogueLines = EndSecondLevelBad; break;
            case 6: dialogueLines = OilLevelGood; break;
            case 7: dialogueLines = OilLevelBad; break;
            default: dialogueLines = firstVisitLines; break;
        }

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogError("dialogueLines не назначены или пусты!");
            dialogueLines = new string[] { "..." };
        }

        if (dialogueText == null)
        {
            Debug.LogError("DialogueText не назначен в инспекторе!");
        }

        if (continueHint != null)
            continueHint.SetActive(true);

        StartTypingLine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLine];
                isTyping = false;
                continueHint.SetActive(true);
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    void StartTypingLine()
    {
        if (dialogueLines == null || currentLine >= dialogueLines.Length) return;

        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));

        if (continueHint != null)
            continueHint.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            if (dialogueText != null)
                dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (continueHint != null)
            continueHint.SetActive(true);
    }

    void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            if (continueHint != null)
                continueHint.SetActive(false);

            StartTypingLine();
        }
        else
        {
            if (dialogueText != null)
                dialogueText.text = "";

            if (continueHint != null)
                continueHint.SetActive(false);

            int stage = PlayerPrefs.GetInt("HQDialogueStage", 1);
            var fader = FindObjectOfType<ScreenFader>();

            if (fader == null)
            {
                Debug.LogError("ScreenFader не найден в сцене!");
                return;
            }

            switch (stage)
            {
                case 1:
                    if (AchievementManager.Instance != null)
                        AchievementManager.Instance.Unlock("Офисные будни");
                    fader.nextSceneName = "TutorialScene";
                    break;
                case 2:
                case 3:
                    fader.nextSceneName = "SecondDay";
                    break;
                case 4:
                case 5:
                    fader.nextSceneName = "OilDay";
                    break;
                case 6:
                case 7:
                    fader.nextSceneName = ""; // если нужно остаться на текущей сцене
                    break;
            }

            fader.FadeOutAndLoadScene();
        }
    }
}
