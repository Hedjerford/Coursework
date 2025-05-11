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

    private void Start()
    {
        StartTypingLine();
        continueHint.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Пропустить печать — показать всю строку сразу
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
            GameObject.FindObjectOfType<ScreenFader>().FadeOutAndLoadScene();

            // Загрузка следующей сцены
            GameObject.FindObjectOfType<ScreenFader>().FadeOutAndLoadScene(); // название сцены точно как в файле
        }
    }
}
