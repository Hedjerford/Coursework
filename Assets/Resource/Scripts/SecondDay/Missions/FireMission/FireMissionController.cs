using UnityEngine;
using System.Collections;

public class FireMissionController : MonoBehaviour
{
    [Header("Объекты")]
    public GameObject fireEffect;
    public Collider2D waterZone;
    public Collider2D fireZone;
    public GameObject additionalFirePrefab;
    public Transform[] fireSpawnPoints;
    public FireMissionUI fireUI;

    [Header("Миссия")]
    public float missionDuration = 60f;
    public int requiredExtinguishes = 10;

    [Header("Диалог и Сергей")]
    public FollowPlayerPathfinding sergeyFollow;
    public Transform playerTarget;
    public SecondDayDialogue dialogue;
    public string[] successDialogueLines;
    public string[] failDialogueLines;
    public static bool FireMissonEnd = false;

    private float timer;
    private int extinguishCount = 0;
    private bool hasWater = false;
    private bool missionStarted = false;
    private bool missionEnded = false;
    private bool nextMissionTriggered = false;

    [Header("Успешность миссии")]
    public static bool SuccessMission;
    void Update()
    {
        if (!missionStarted || missionEnded)
            return;

        timer -= Time.deltaTime;
        fireUI?.SetTime(timer);

        if (timer <= 0f && !missionEnded)
        {
            missionEnded = true;
            if (extinguishCount < requiredExtinguishes)
                FailMission();
            else
                CompleteMission();
        }

        Vector2 playerPos = FindObjectOfType<PlayerMovement>().transform.position;

        if (waterZone.OverlapPoint(playerPos) && !hasWater)
            InteractionHintController.Instance?.ShowHint(true);
        else if (fireZone.OverlapPoint(playerPos) && hasWater)
            InteractionHintController.Instance?.ShowHint(true);
        else
            InteractionHintController.Instance?.ShowHint(false);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (waterZone.OverlapPoint(playerPos) && !hasWater)
            {
                hasWater = true;
                Debug.Log("💧 Вода набрана");
            }
            else if (fireZone.OverlapPoint(playerPos) && hasWater)
            {
                hasWater = false;
                extinguishCount++;
                Debug.Log($"🔥 Потушено {extinguishCount}/{requiredExtinguishes}");
                fireUI?.SetProgress(extinguishCount, requiredExtinguishes);

                if (extinguishCount >= requiredExtinguishes && !missionEnded)
                {
                    missionEnded = true;
                    CompleteMission();
                }
            }
        }
    }

    public void StartMission()
    {
        if (missionStarted)
        {
            Debug.LogWarning("🔥 Миссия уже была запущена");
            return;
        }

        missionStarted = true;
        missionEnded = false;
        nextMissionTriggered = false;
        extinguishCount = 0;
        hasWater = false;
        timer = missionDuration;

        fireEffect.SetActive(true);

        if (fireUI != null)
        {
            fireUI.SetVisible(true);
            fireUI.SetProgress(0, requiredExtinguishes);
            fireUI.SetTime(missionDuration);
        }

        Debug.Log("🚨 Миссия тушения пожара началась");
    }

    private void CompleteMission()
    {
        SuccessMission = true;
        FireMissonEnd = true;
        FindObjectOfType<LevelCompletionManager_SecondDay>()?.OnFireTimerEnd();
        Debug.Log("✅ Пожар потушен успешно");

        missionStarted = false;
        fireEffect.SetActive(false);
        fireUI?.SetVisible(false);
        InteractionHintController.Instance?.ShowHint(false);

        StartCoroutine(SergeyDialogueThenNext(successDialogueLines));
        AchievementManager.Instance.Unlock("Пожарные со стажем");;
        LevelCompletionManager_SecondDay manager = FindObjectOfType<LevelCompletionManager_SecondDay>();
        FindObjectOfType<LevelCompletionManager_SecondDay>()?.CheckCompletion();

    }

    private void FailMission()
    {

        SuccessMission = false;
        FireMissonEnd = true;
        FindObjectOfType<LevelCompletionManager_SecondDay>()?.OnFireTimerEnd();
        AchievementManager.Instance.Unlock("Неудача...");
        Debug.Log("❌ Пожар не потушен — миссия провалена");
        missionStarted = false;
        fireUI?.SetVisible(false);
        InteractionHintController.Instance?.ShowHint(false);

        // Расширение огня
        if (fireZone is BoxCollider2D box)
        {
            box.size *= 0.5f;
            box.offset = Vector2.zero;
            box.isTrigger = false;

            Vector2 size = box.size;
            Vector2 center = box.bounds.center;
            float step = 2f;
            int count = 0;

            for (float x = -size.x / 2f; x <= size.x / 2f; x += step)
            {
                for (float y = -size.y / 2f; y <= size.y / 2f; y += step)
                {
                    Vector2 spawnPos = center + new Vector2(x, y);
                    Instantiate(additionalFirePrefab, spawnPos, Quaternion.identity);
                    count++;
                    if (count >= 200) break;
                }
                if (count >= 200) break;
                LevelCompletionManager_SecondDay manager = FindObjectOfType<LevelCompletionManager_SecondDay>();
                if (manager != null)
                    manager.Invoke("CheckCompletion", 1f); // можно с задержкой
            }

            Debug.Log($"🔥 Заспавнено {count} очагов огня");
        }

        StartCoroutine(SergeyDialogueThenNext(failDialogueLines));
    }
    
    private IEnumerator SergeyDialogueThenNext(string[] lines)
    {
        if (nextMissionTriggered)
            yield break;

        nextMissionTriggered = true;

        // Сергей подходит
        sergeyFollow.SetStoppingDistance(0.2f);
        sergeyFollow.SetTarget(playerTarget);

        while (Vector3.Distance(sergeyFollow.transform.position, playerTarget.position) > 0.5f)
            yield return null;

        sergeyFollow.EnableMovement(false);

        // Диалог
        dialogue.StartCustomDialogue(lines);

        while (!dialogue.IsDialogueFinished)
            yield return null;

        // Запуск следующей миссии
        Debug.Log("➡️ Диалог завершён. Запуск следующей миссии.");
        NextMissionManager.Instance?.StartNext();
    }
}
