using UnityEngine;
using UnityEngine.AI;

public class SergeyIntroDialogue : MonoBehaviour
{
    public float approachSpeed = 2f;
    public float approachStoppingDistance = 1.5f;
    [TextArea(3, 10)] public string[] introLines;

    private Transform player;
    private NavMeshPath path;
    private int currentCorner = 0;
    private bool isMoving = true;
    private bool hasStartedDialogue = false;

    private EmergencyDialogue dialogueUI;
    private FollowPlayerPathfinding followScript;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        dialogueUI = FindObjectOfType<EmergencyDialogue>();
        followScript = GetComponent<FollowPlayerPathfinding>();

        if (followScript != null)
            followScript.EnableMovement(false);

        path = new NavMeshPath();
        UpdatePathToPlayer();
    }

    void Update()
    {
        if (!isMoving || player == null) return;

        if (Vector3.Distance(transform.position, player.position) <= approachStoppingDistance)
        {
            isMoving = false;
            StartIntroDialogue();
            return;
        }

        FollowPathToPlayer();
        LockZ();
    }

    void FollowPathToPlayer()
    {
        if (path == null || path.corners.Length == 0) return;

        Vector3 target = path.corners[currentCorner];
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * approachSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.3f)
        {
            currentCorner++;
            if (currentCorner >= path.corners.Length)
                UpdatePathToPlayer();
        }
    }

    void UpdatePathToPlayer()
    {
        if (player == null) return;

        if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path))
            currentCorner = 0;
    }

    void StartIntroDialogue()
    {
        if (hasStartedDialogue || dialogueUI == null || introLines.Length == 0) return;

        hasStartedDialogue = true;
        dialogueUI.StartDialogueLines(introLines, OnDialogueFinished);
    }

    void OnDialogueFinished()
    {
        Debug.Log("🚶‍♂️ Сергей: начинаю следовать за игроком");

        if (followScript != null)
        {
            followScript.SetTarget(player);
            followScript.SetStoppingDistance(approachStoppingDistance);
            followScript.EnableMovement(true);
        }
    }

    void LockZ()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
