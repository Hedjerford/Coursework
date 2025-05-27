using UnityEngine;
using UnityEngine.AI;

public class EmergencyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float stoppingDistance = 0.1f;
    public float playerDetectRadius = 1.5f;
    public float dialogueCooldown = 5f;

    private int currentPoint = 0;
    private NavMeshPath path;
    private int currentCorner = 0;
    private Transform player;
    private bool isTalking = false;
    private bool canTalk = true;

    private EmergencyDialogue dialogueUI;

    void Start()
    {
        path = new NavMeshPath();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        dialogueUI = FindObjectOfType<EmergencyDialogue>();

        SetPathTo(patrolPoints[currentPoint].position);
    }

    void Update()
    {
        if (isTalking) return;

        if (canTalk && Vector3.Distance(transform.position, player.position) < playerDetectRadius)
        {
            isTalking = true;
            canTalk = false;

            if (dialogueUI != null)
                dialogueUI.StartDialogue(ResumePatrol);

            return;
        }

        FollowPath();
        LockZPosition();
    }

    void FollowPath()
    {
        if (path == null || path.corners.Length == 0) return;

        Vector3 target = path.corners[currentCorner];
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) <= stoppingDistance)
        {
            currentCorner++;
            if (currentCorner >= path.corners.Length)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                SetPathTo(patrolPoints[currentPoint].position);
            }
        }
    }

    void SetPathTo(Vector3 targetPosition)
    {
        if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            currentCorner = 0;
        }
    }

    void ResumePatrol()
    {
        isTalking = false;
        SetPathTo(patrolPoints[currentPoint].position);
        Invoke(nameof(EnableTalking), dialogueCooldown);
    }

    void EnableTalking()
    {
        canTalk = true;
    }

    void LockZPosition()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
