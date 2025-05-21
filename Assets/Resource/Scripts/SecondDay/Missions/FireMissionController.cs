using UnityEngine;
using System.Collections;

public class FireMissionController : MonoBehaviour
{
    public GameObject fireEffect;
    public Collider2D waterZone;
    public Collider2D fireZone;

    public GameObject additionalFirePrefab;
    public Transform[] fireSpawnPoints;

    public float missionDuration = 60f;
    public int requiredExtinguishes = 10;

    public FireMissionUI fireUI;

    public FollowPlayerPathfinding sergeyFollow;
    public Transform playerTarget;
    public SecondDayDialogue dialogue;
    public string[] successDialogueLines;
    public string[] failDialogueLines;

    private float timer;
    private int extinguishCount = 0;
    private bool hasWater = false;
    private bool missionStarted = false;
    private bool missionFailed = false;

    void Update()
    {
        if (fireUI != null)
            fireUI.SetTime(timer);

        if (!missionStarted || missionFailed)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
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
                Debug.Log("\uD83D\uDCA7 Вода набрана");
            }
            else if (fireZone.OverlapPoint(playerPos) && hasWater)
            {
                hasWater = false;
                extinguishCount++;
                Debug.Log($"\uD83D\uDD25 Потушено {extinguishCount}/{requiredExtinguishes}");

                if (fireUI != null)
                    fireUI.SetProgress(extinguishCount, requiredExtinguishes);

                if (extinguishCount >= requiredExtinguishes)
                    CompleteMission();
            }
        }
    }

    public void StartMission()
    {
        missionStarted = true;
        missionFailed = false;
        timer = missionDuration;
        extinguishCount = 0;
        hasWater = false;

        fireEffect.SetActive(true);

        if (fireUI != null)
        {
            fireUI.SetVisible(true);
            fireUI.SetProgress(0, requiredExtinguishes);
            fireUI.SetTime(missionDuration);
        }
    }

    private void CompleteMission()
    {
        missionStarted = false;
        fireEffect.SetActive(false);
        InteractionHintController.Instance?.ShowHint(false);

        if (fireUI != null)
        {
            fireUI.SetProgress(extinguishCount, requiredExtinguishes);
            fireUI.SetVisible(false);
        }

        sergeyFollow.SetStoppingDistance(0.2f);
        StartCoroutine(WaitForSergeyThenStartDialogue(successDialogueLines));
        StartCoroutine(StartNextMissionAfterDialogue());
    }

    private void FailMission()
    {
        missionStarted = false;
        missionFailed = true;
        InteractionHintController.Instance?.ShowHint(false);

        if (fireUI != null)
            fireUI.SetVisible(false);

        Debug.Log("❌ Миссия провалена. Пожар вышел из-под контроля");

        var box = fireZone as BoxCollider2D;
        if (box != null)
        {
            box.size *= 2f;
            box.offset = Vector2.zero;
            box.isTrigger = false;

            Vector2 size = box.size;
            Vector2 center = box.bounds.center;
            float step = 2f;
            int maxFires = 200;
            int count = 0;

            for (float x = -size.x / 2f; x <= size.x / 2f; x += step)
            {
                for (float y = -size.y / 2f; y <= size.y / 2f; y += step)
                {
                    Vector2 spawnPos = center + new Vector2(x, y);
                    Instantiate(additionalFirePrefab, spawnPos, Quaternion.identity);

                    count++;
                    if (count >= maxFires)
                        break;
                }
                if (count >= maxFires)
                    break;
            }

            Debug.Log($"\uD83D\uDD25 Заспавнено {count} очагов огня по зоне {box.size}");
        }

        sergeyFollow.SetStoppingDistance(0.2f);
        StartCoroutine(WaitForSergeyThenStartDialogue(failDialogueLines));
        StartCoroutine(StartNextMissionAfterDialogue());
    }

    private IEnumerator WaitForSergeyThenStartDialogue(string[] lines)
    {
        Transform sergey = sergeyFollow.transform;
        Vector3 target = playerTarget.position;

        // Если Сергей уже рядом — запускаем диалог сразу
        if (Vector3.Distance(sergey.position, target) <= 0.3f)
        {
            if (dialogue != null)
                dialogue.StartCustomDialogue(lines);

            yield break;
        }

        // Иначе ждём подхода
        while (Vector3.Distance(sergey.position, target) > 0.3f)
        {
            yield return null;
        }

        if (dialogue != null)
            dialogue.StartCustomDialogue(lines);
    }
    private IEnumerator StartNextMissionAfterDialogue()
    {
        yield return new WaitForSeconds(2f); // Лучше заменить на проверку завершения диалога

        Debug.Log("▶ Запускаем следующую миссию...");
        // Пример: можно вызвать сцену, триггер или менеджер
        NextMissionManager.Instance?.StartNext();
    }
}
