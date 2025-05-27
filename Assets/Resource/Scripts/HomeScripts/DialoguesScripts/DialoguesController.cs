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

    [Header("ƒиалоги")]
    [TextArea(3, 10)] public string[] firstVisitLines;
    [TextArea(3, 10)] public string[] BadDialogue; // ≈сли игрок плохо выполнил задание
    [TextArea(3, 10)] public string[] GoodDialogue; // ≈сли игрок выполнил задание хорошо
    [TextArea(3, 10)] public string[] EndSecondLevelGood;
    [TextArea(3, 10)] public string[] EndSecondLevelBad;
    [TextArea(3, 10)] public string[] OilLevelGood;
    [TextArea(3, 10)] public string[] OilLevelBad;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("HasLaunched"))
        {
            PlayerPrefs.SetInt("HQDialogueStage", 1); // начальный диалог
            PlayerPrefs.SetInt("HasLaunched", 1);     // пометка, что уже запускали
            PlayerPrefs.Save();
            
        }
  

    int stage = PlayerPrefs.GetInt("HQDialogueStage", 1); // по умолчанию 1

        switch (stage)
        {
            case 1:
                dialogueLines = firstVisitLines;
                break;
            case 2:
                dialogueLines = BadDialogue;
                break;
            case 3:
                dialogueLines = GoodDialogue;
                break;
            case 4:
                dialogueLines = EndSecondLevelGood;
                break;
            case 5:
                dialogueLines = EndSecondLevelBad;
                break;
            case 6:
                dialogueLines = OilLevelGood;
                break;
            case 7:
                dialogueLines = OilLevelBad;
                break;
            default:
                dialogueLines = firstVisitLines;
                break;
        }

        StartTypingLine();
        continueHint.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // ѕропустить печать Ч показать всю строку сразу
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLine];
                isTyping = false;
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    void StartTypingLine()
    {
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));
        continueHint.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueHint.SetActive(true);
    }

    void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            continueHint.SetActive(false);

            StartTypingLine();
        }
        else
        {
            dialogueText.text = "";
            continueHint.SetActive(false);

            int stage = PlayerPrefs.GetInt("HQDialogueStage", 1);
            var fader = FindObjectOfType<ScreenFader>();

            if (stage == 1)
            {
                fader.nextSceneName = "TutorialScene"; 
                fader.FadeOutAndLoadScene();
            }
            else if (stage == 2 || stage == 3 )
            {
                fader.nextSceneName = "SecondDay"; 
                fader.FadeOutAndLoadScene();
            }
            else if (stage == 4 || stage == 5)
            {
                fader.nextSceneName = "OilDay";
                fader.FadeOutAndLoadScene();
            }
        }

    }
}
