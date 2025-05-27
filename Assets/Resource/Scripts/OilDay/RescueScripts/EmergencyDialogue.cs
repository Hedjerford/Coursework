using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EmergencyDialogue : MonoBehaviour
{
    [Header("UI")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDuration = 0.5f;

    public TextMeshProUGUI dialogueText;
    public GameObject continueHint;

    [Header("Фразы МЧС")]
    [TextArea(3, 10)] public string[] possibleLines;
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool dialogueActive = false;

    // Поддержка всех форматов
    private string[] currentLines;
    private int currentIndex = 0;

    private System.Action onDialogueFinished;
    private PlayerMovement player;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentIndex];
                isTyping = false;
                continueHint.SetActive(true);
            }
            else
            {
                currentIndex++;
                if (currentIndex < currentLines.Length)
                {
                    StartTypingLine();
                }
                else
                {
                    StartCoroutine(SlideBarsOut());
                }
            }
        }
    }

    // 📌 Случайная фраза для МЧС
    public void StartDialogue(System.Action onFinish = null)
    {
        if (possibleLines.Length == 0)
        {
            Debug.LogWarning("⚠️ Нет фраз для МЧСника");
            return;
        }

        string randomLine = possibleLines[Random.Range(0, possibleLines.Length)];
        StartDialogueLine(randomLine, onFinish);
    }

    // 📌 Одна строка
    public void StartDialogueLine(string line, System.Action onFinish = null)
    {
        StartDialogueLines(new string[] { line }, onFinish);
    }

    // 📌 Несколько строк (для Сергея и кат-сцен)
    public void StartDialogueLines(string[] lines, System.Action onFinish = null)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("⚠️ Передан пустой массив строк");
            return;
        }

        currentLines = lines;
        currentIndex = 0;
        onDialogueFinished = onFinish;
        dialogueActive = true;

        if (player != null)
            player.DisableMovement();

        StartCoroutine(SlideBarsIn());
    }

    IEnumerator SlideBarsIn()
    {
        float t = 0f;
        Vector2 topStart = new Vector2(0, topBar.rect.height);
        Vector2 botStart = new Vector2(0, -bottomBar.rect.height);
        Vector2 target = Vector2.zero;

        while (t < barSlideDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / barSlideDuration);
            topBar.anchoredPosition = Vector2.Lerp(topStart, target, progress);
            bottomBar.anchoredPosition = Vector2.Lerp(botStart, target, progress);
            yield return null;
        }

        topBar.anchoredPosition = target;
        bottomBar.anchoredPosition = target;

        StartTypingLine();
    }

    void StartTypingLine()
    {
        dialogueText.text = "";
        continueHint.SetActive(false);
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        continueHint.SetActive(true);
    }

    IEnumerator SlideBarsOut()
    {
        dialogueActive = false;

        float t = 0f;
        Vector2 topEnd = new Vector2(0, topBar.rect.height);
        Vector2 botEnd = new Vector2(0, -bottomBar.rect.height);
        Vector2 start = Vector2.zero;

        while (t < barSlideDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / barSlideDuration);
            topBar.anchoredPosition = Vector2.Lerp(start, topEnd, progress);
            bottomBar.anchoredPosition = Vector2.Lerp(start, botEnd, progress);
            yield return null;
        }

        topBar.anchoredPosition = topEnd;
        bottomBar.anchoredPosition = botEnd;

        dialogueText.text = "";
        continueHint.SetActive(false);

        if (player != null)
            player.EnableMovement();

        onDialogueFinished?.Invoke();
        onDialogueFinished = null;

        Debug.Log("🗨 Диалог завершён");
    }
}
