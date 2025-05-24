using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SecondDayDialogue : MonoBehaviour
{
    [TextArea(3, 10)] public string[] dialogueLines;
    public TextMeshProUGUI dialogueText;
    public GameObject continueHint;

    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDuration = 0.5f;
    public float typingSpeed = 0.03f;

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool dialogueFinished = false;
    private bool isInitialCutscene = true;

    private PlayerMovement player;
    public FireMissionController fireMission;
    public bool IsDialogueFinished => dialogueFinished;

    [Header("Зверьки")]
    public GameObject[] animalPrefabs;
    public Transform[] animalSpawnPoints;
    public Transform[] firstTargets;             // ⬅️ новое: промежуточные цели
    public Transform[] trapTargets;
    public int animalsToSpawn = 5;
    public bool EndSpawn = true;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.DisableMovement();

        StartCoroutine(StartCutscene());
    }

    private IEnumerator StartCutscene()
    {
        yield return StartCoroutine(SlideBarsIn());
        StartTypingLine();
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

    public void StartCustomDialogue(string[] lines)
    {
        Debug.Log($"📢 Запускаем кастомный диалог с {lines.Length} строками");

        dialogueLines = lines;
        currentLine = 0;
        dialogueFinished = false;
        isInitialCutscene = false;

        if (player == null)
            player = FindObjectOfType<PlayerMovement>();

        player?.DisableMovement();

        StartCoroutine(SlideBarsIn());
        StartTypingLine();
    }

    private void Update()
    {
        if (!dialogueFinished && Input.GetKeyDown(KeyCode.Space))
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

    void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            StartTypingLine();
        }
        else
        {
            StartCoroutine(SlideBarsOut());
        }
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
        if(FireMissionController.FireMissonEnd)
        {
            SpawnAnimals(); // 🐾 Спавним зверей сразу после появления панелей
        }
        
    }

    IEnumerator SlideBarsOut()
    {
        if (player != null)
            player.EnableMovement();

        var sergey = FindObjectOfType<FollowPlayerPathfinding>();
        if (sergey != null)
        {
            sergey.SetStoppingDistance(1.5f);
            sergey.EnableMovement(true);
        }

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

        dialogueFinished = true;

        if (player != null)
            player.EnableMovement();

        AchievementManager.Instance.Unlock("Второй день");

        if (isInitialCutscene && fireMission != null)
            fireMission.StartMission();
    }

    private void SpawnAnimals()
    {
        Debug.Log("🐾 Спавним зверьков по маршруту");

        List<Transform> availableTraps = new List<Transform>(trapTargets);

        for (int i = 0; i < animalsToSpawn; i++)
        {
            if (availableTraps.Count == 0)
            {
                Debug.LogWarning("⚠️ Не хватает капканов для всех зверей!");
                break;
            }

            GameObject prefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
            Transform spawn = animalSpawnPoints[Random.Range(0, animalSpawnPoints.Length)];
            Transform first = firstTargets[Random.Range(0, firstTargets.Length)];
            int trapIndex = Random.Range(0, availableTraps.Count);
            Transform trap = availableTraps[trapIndex];
            availableTraps.RemoveAt(trapIndex); // 🚫 больше не используем этот капкан

            GameObject animal = Instantiate(prefab, spawn.position, Quaternion.identity);

            AnimalMover mover = animal.GetComponent<AnimalMover>();
            if (mover != null)
            {
                mover.firstTarget = first;
                mover.finalTrap = trap;
            }
            else
            {
                Debug.LogWarning("❗ На префабе зверька нет AnimalMover!");
            }
        }
    }
}
