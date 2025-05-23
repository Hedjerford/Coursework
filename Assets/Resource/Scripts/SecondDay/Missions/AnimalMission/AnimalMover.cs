using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalMover : MonoBehaviour
{
    public float speed = 2f;
    public float repathInterval = 3f;
    public Transform[] trapTargets; // потенциальные капканы

    private Rigidbody2D rb;
    private NavMeshPath path;
    private int currentPathIndex;
    private float repathTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        path = new NavMeshPath();
        repathTimer = repathInterval;
        SetRandomTrapPath();
    }

    void Update()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            SetRandomTrapPath();
        }

        MoveAlongPath();
    }

    void SetRandomTrapPath()
    {
        if (trapTargets.Length == 0) return;

        Transform target = trapTargets[Random.Range(0, trapTargets.Length)];
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            currentPathIndex = 0;
        }
    }

    void MoveAlongPath()
    {
        if (path == null || path.corners.Length == 0 || currentPathIndex >= path.corners.Length)
            return;

        Vector3 targetPos = path.corners[currentPathIndex];
        Vector3 moveDir = (targetPos - transform.position).normalized;
        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            currentPathIndex++;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Debug.Log("🐾 Зверёк попал в капкан!");
            Destroy(gameObject); // или проиграть анимацию
        }
    }
}
