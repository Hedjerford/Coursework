using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerPathfinding : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stoppingDistance = 0.5f;

    public Animator animator; // 👈 Ссылка на Animator
    public string horizontalParam = "Horizontal";
    public string verticalParam = "Vertical";

    private NavMeshPath path;
    private int currentCorner = 0;
    private bool canMove = true;

    void Start()
    {
        path = new NavMeshPath();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
        UpdatePath();
    }

    public void SetStoppingDistance(float newDistance)
    {
        stoppingDistance = newDistance;
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;

        if (!enable && animator != null)
        {
            animator.SetFloat(horizontalParam, 0f);
            animator.SetFloat(verticalParam, 0f);
        }
    }

    void UpdatePath()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path);
            currentCorner = 0;
        }
    }

    void Update()
    {
        if (!canMove || path == null || path.corners.Length == 0 || currentCorner >= path.corners.Length)
        {
            if (animator != null)
            {
                animator.SetFloat(horizontalParam, 0f);
                animator.SetFloat(verticalParam, 0f);
            }
            return;
        }

        Vector3 target = path.corners[currentCorner];
        target.z = 0f;

        Vector3 direction = (target - transform.position).normalized;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.5f)
            {
                currentCorner++;
            }
        }
        else
        {
            direction = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetFloat(horizontalParam, direction.x);
            animator.SetFloat(verticalParam, direction.y);
        }

        transform.rotation = Quaternion.identity;
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
