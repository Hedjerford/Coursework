using UnityEngine;
using System.Collections;

public class Mission2Controller : MonoBehaviour
{
    [Header("Точки назначения")]
    public Transform fireTargetPoint;    // куда идут после диалога
    public Transform playerTarget;       // куда подходят для диалога

    [Header("Диалог")]
    public SecondDayDialogue dialogue;
    public string[] successDialogueLines;
    public string[] failDialogueLines;
    public bool playerSucceededPreviously = true;

    [Header("МЧС")]
    public GameObject rescueTeamPrefab;
    public Transform spawnPoint;
    private RescueTeamPathfinder rescueTeam;

    [Header("Настройки")]
    public float dialogueStartDistance = 1.5f;

    private bool started = false;

    public void SetPlayerSucceeded(bool succeeded)
    {
        playerSucceededPreviously = succeeded;
    }
    private void Start()
    {
        StartMission();
    }
    public void StartMission()
    {
        if (started)
        {
            Debug.LogWarning("🔁 Миссия с МЧС уже была запущена.");
            return;
        }

        started = true;

        Debug.Log("🚒 Спавним МЧС и направляем к игроку");

        GameObject obj = Instantiate(rescueTeamPrefab, spawnPoint.position, Quaternion.identity);
        rescueTeam = obj.GetComponent<RescueTeamPathfinder>();

        if (rescueTeam == null)
        {
            Debug.LogError("❌ На префабе нет компонента RescueTeamPathfinder!");
            return;
        }

        rescueTeam.EnableMovement(true);
        rescueTeam.SetTarget(playerTarget);

        StartCoroutine(WaitForArrivalAtPlayer());
    }

    private IEnumerator WaitForArrivalAtPlayer()
    {
        while (Vector3.Distance(rescueTeam.transform.position, playerTarget.position) > dialogueStartDistance)
            yield return null;

        Debug.Log("✅ МЧС подошёл к игроку. Запускаем диалог");

        rescueTeam.EnableMovement(false); // Останавливаем движение на время диалога

        var lines = FireMissionController.SuccessMission ? successDialogueLines : failDialogueLines;
        dialogue.StartCustomDialogue(lines);


        yield return StartCoroutine(WaitForDialogueThenGoToFire());
    }

    private IEnumerator WaitForDialogueThenGoToFire()
    {
        while (!dialogue.IsDialogueFinished)
            yield return null;

        Debug.Log("➡️ Диалог завершён. МЧС направляется к пожару");

        rescueTeam.EnableMovement(true); // Включаем движение
        rescueTeam.SetTarget(fireTargetPoint);
    }
}
