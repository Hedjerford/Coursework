using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Mission2Controller : MonoBehaviour
{
    [Header("Объекты и перемещение")]
    public Transform rescueArrivalPoint;
    public Transform fireTargetPoint;

    public FollowPlayerPathfinding sergeyFollow;
    public Transform playerTarget;

    [Header("Диалог")]
    public SecondDayDialogue dialogue;
    public string[] successDialogueLines;
    public string[] failDialogueLines;
    public string[] sergeyDialogueLines;
    public bool playerSucceededPreviously = true; // ← передаётся из предыдущей миссии

    [Header("Спавн зверей")]
    public GameObject animalsSpawner;

    [Header("Спавн МЧС")]
    public GameObject rescueTeamPrefab;
    public Transform spawnPoint;
    private RescueTeamPathfinder rescueTeam;

    public void StartMission()
    {
        Debug.Log("✅ Спавним МЧС в точке: " + spawnPoint.position);
        Debug.Log("🎯 Цель: " + rescueArrivalPoint.position);
        Debug.Log("▶ Компонент RescueTeamPathfinder: " + rescueTeam);
        GameObject obj = Instantiate(rescueTeamPrefab, spawnPoint.position, Quaternion.identity);
        rescueTeam = obj.GetComponent<RescueTeamPathfinder>();

        if (rescueTeam == null)
        {
            Debug.LogError("❌ На префабе нет RescueTeamPathfinder!");
            return;
        }

        Debug.Log("🚒 МЧС выехали");
        rescueTeam.SetTarget(rescueArrivalPoint);
        StartCoroutine(RescueArrival());
    }

    private IEnumerator RescueArrival()
    {
        while (Vector3.Distance(rescueTeam.transform.position, rescueArrivalPoint.position) > 0.5f)
            yield return null;

        Debug.Log("✅ МЧС прибыл. Начинаем диалог.");
        var lines = playerSucceededPreviously ? successDialogueLines : failDialogueLines;
        dialogue.StartCustomDialogue(lines);

        yield return StartCoroutine(WaitForDialogueThenMoveToFire());
    }

    private IEnumerator WaitForDialogueThenMoveToFire()
    {
        yield return new WaitForSeconds(1f);
        while (!dialogue.IsDialogueFinished)
            yield return null;

        Debug.Log("🔥 МЧС направляется к пожару");
        rescueTeam.SetTarget(fireTargetPoint);

        yield return new WaitForSeconds(1f); // буфер
        StartCoroutine(StartSergeySequence());
    }

    private IEnumerator StartSergeySequence()
    {
        Debug.Log("🚶‍♂️ Сергей подходит к игроку");
        sergeyFollow.SetStoppingDistance(0.2f);
        sergeyFollow.SetTarget(playerTarget);

        while (Vector3.Distance(sergeyFollow.transform.position, playerTarget.position) > 0.3f)
            yield return null;

        sergeyFollow.EnableMovement(false);
        dialogue.StartCustomDialogue(sergeyDialogueLines);

        yield return new WaitForSeconds(1f);
        while (!dialogue.IsDialogueFinished)
            yield return null;

        Debug.Log("🐾 Запуск зверей...");
        animalsSpawner?.SetActive(true);
    }
}
