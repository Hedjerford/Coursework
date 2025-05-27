using UnityEngine;
using System.Collections.Generic;

public class BoomPlacementController : MonoBehaviour
{
    public static BoomPlacementController Instance;

    [Header("Настройки миссии")]
    public GameObject boomPrefab;
    public int totalBoomsToPlace = 6;
    public int additionalBoomsPerSegment = 2;
    public float placementRadius = 3.5f;
    public float missionDuration = 60f;

    private List<GameObject> manualBooms = new List<GameObject>();
    private bool canPlace = false;
    private Vector3 currentPlacementPosition;
    public static bool TimerEnd = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!BoomZoneTrigger.Instance?.IsPlayerInside() ?? true) return;
        if (manualBooms.Count >= totalBoomsToPlace) return;
        if (!canPlace) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaceBoomAtCurrentSpot();
        }
    }

    public void SetPlacementAllowed(bool allowed, Vector3 position)
    {
        canPlace = allowed;
        currentPlacementPosition = position;
    }

    private void PlaceBoomAtCurrentSpot()
    {
        Vector3 pos = currentPlacementPosition;
        pos.z = 0f;

        GameObject boom = Instantiate(boomPrefab, pos, Quaternion.identity);
        manualBooms.Add(boom);

        canPlace = false;
        InteractionHintController.Instance?.ShowHint(false);
        BoomMissionUI.Instance?.UpdateBoomCount(manualBooms.Count);

        Debug.Log($"📍 Установлено вручную: {manualBooms.Count}/{totalBoomsToPlace}");

        if (manualBooms.Count == totalBoomsToPlace)
        {
            FinalizeCircle();
        }
    }

    private void FinalizeCircle()
    {
        Debug.Log("✅ Все боны установлены вручную. Строим круг...");

        Vector3 center = BoomZoneTrigger.Instance.GetZoneCenter();
        float angleStep = 360f / totalBoomsToPlace;

        // Расставляем вручную установленные по кругу
        for (int i = 0; i < manualBooms.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * placementRadius;
            pos.z = 0f;

            Transform boom = manualBooms[i].transform;
            boom.position = pos;

            RotateTangentialToCircle(boom, center);
        }

        // Вставляем дополнительные боны между основными
        for (int i = 0; i < manualBooms.Count; i++)
        {
            Vector3 a = manualBooms[i].transform.position;
            Vector3 b = manualBooms[(i + 1) % manualBooms.Count].transform.position;

            for (int j = 1; j <= additionalBoomsPerSegment; j++)
            {
                float t = j / (float)(additionalBoomsPerSegment + 1);
                Vector3 pos = Vector3.Lerp(a, b, t);
                pos.z = 0f;

                GameObject boom = Instantiate(boomPrefab, pos, Quaternion.identity);
                RotateTangentialToCircle(boom.transform, center);
            }
        }
        FindObjectOfType<LevelCompletionManager_Simple>()?.ShowCompletionPanel();
        Debug.Log("🟢 Круг завершён с дополнительными бонами.");

        // Удаляем все точки установки
        BoomPlacementPoint[] points = FindObjectsOfType<BoomPlacementPoint>();
        foreach (var point in points)
        {
            Destroy(point.gameObject);
        }

        // Прячем UI
        BoomMissionUI.Instance?.Hide();
    }

    private void RotateTangentialToCircle(Transform obj, Vector3 center)
    {
        Vector2 dirFromCenter = (obj.position - center).normalized;
        float angle = Mathf.Atan2(dirFromCenter.y, dirFromCenter.x) * Mathf.Rad2Deg;
        angle += 90f; // если спрайт смотрит вверх
        obj.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void OnTimerEnd()
    {
       
        Debug.Log("❌ Время вышло! Показываем панель завершения.");

        BoomMissionUI.Instance?.Hide();

        // Показываем обычную завершённую панель
        FindObjectOfType<LevelCompletionManager_Simple>()?.ShowCompletionPanel();
        TimerEnd = true;
        Debug.Log(TimerEnd);

    }

    public void StartBoomMission()
    {
        BoomMissionUI.Instance?.StartMissionUI(totalBoomsToPlace, missionDuration);
        Debug.Log("▶ Миссия с бонами официально началась.");
    }

}
