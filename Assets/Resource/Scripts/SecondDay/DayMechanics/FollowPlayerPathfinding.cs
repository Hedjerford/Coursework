using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerPathfinding : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stoppingDistance = 1.5f; // минимальна€ дистанци€

    private NavMeshPath path;
    private int currentCorner = 0;

    void Start()
    {
        path = new NavMeshPath();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
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
        if (path == null || path.corners.Length == 0 || currentCorner >= path.corners.Length)
            return;

        Vector3 target = path.corners[currentCorner];
        target.z = 0f;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // “олько если дальше, чем дистанци€ остановки
        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                currentCorner++;
            }
        }

        // —брос поворота и Z
        transform.rotation = Quaternion.identity;
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
