using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RescueTeamPathfinder : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float stoppingDistance = 0.5f;

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
        target = newTarget;

        // Безопасность: path может быть null, если вызвали до Start()
        if (path == null)
            path = new NavMeshPath();

        UpdatePath();
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;

        // Отключить столкновения, если нужно
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = enable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = enable;
    }

    private void UpdatePath()
    {
        if (path == null)
            path = new NavMeshPath();

        if (target != null && Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            currentCorner = 0;
        }
    }

    void Update()
    {
        if (!canMove || path == null || path.corners.Length == 0 || currentCorner >= path.corners.Length)
            return;

        Vector3 targetCorner = path.corners[currentCorner];
        targetCorner.z = 0f;

        transform.position = Vector3.MoveTowards(transform.position, targetCorner, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetCorner) < 0.1f)
            currentCorner++;

        // Зафиксировать Z
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        transform.rotation = Quaternion.identity; // без поворота
    }

    public bool IsNearTarget()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= stoppingDistance;
    }
}
