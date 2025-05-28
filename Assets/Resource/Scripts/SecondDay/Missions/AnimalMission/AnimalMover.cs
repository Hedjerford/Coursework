using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalMover : MonoBehaviour
{
    public float speed = 2f;

    [HideInInspector] public Transform firstTarget;
    [HideInInspector] public Transform finalTrap;

    private Rigidbody2D rb;
    private NavMeshPath path;
    private int currentPathIndex = 0;
    private bool goingToTrap = false;
    private bool isTrapped = false;
    private bool playerNearby = false;
    private bool isRescued = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        path = new NavMeshPath();

        if (firstTarget != null)
            SetPathTo(firstTarget);
    }

    void Update()
    {
        if (isTrapped && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ReleaseFromTrap();
        }

        if (!isTrapped && !isRescued)
        {
            MoveAlongPath();
        }
    }

    void SetPathTo(Transform target)
    {
        if (target == null) return;

        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            currentPathIndex = 0;
    }

    void MoveAlongPath()
    {
        if (path == null || path.corners.Length == 0 || currentPathIndex >= path.corners.Length)
            return;

        Vector3 targetPos = path.corners[currentPathIndex];
        Vector3 moveDir = (targetPos - transform.position).normalized;

        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentPathIndex++;

            if (currentPathIndex >= path.corners.Length)
            {
                if (!goingToTrap)
                {
                    goingToTrap = true;
                    SetPathTo(finalTrap);
                }
            }
        }
    }

    // Ловушка может остаться триггером
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && Vector3.Distance(other.transform.position, finalTrap.position) < 0.5f)
        {
            if (!isTrapped)
            {
                Debug.Log("🐾 Зверёк попал в капкан");
                isTrapped = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // Касание игрока через физику
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    public void ReleaseFromTrap()
    {
        Debug.Log("🆓 Зверёк освобождён!");

        isTrapped = false;
        isRescued = true;
        goingToTrap = false;

        // Случайная точка побега
        Vector3 randomDirection = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        Vector3 candidatePosition = transform.position + randomDirection;

        if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            GameObject temp = new GameObject("EscapeTarget");
            temp.transform.position = hit.position;
            SetPathTo(temp.transform);
        }
        else
        {
            Debug.LogWarning("❗ Не удалось найти точку побега на NavMesh");
        }

        if (AnimalRescueManager.Instance != null)
            AnimalRescueManager.Instance.RegisterRescue();
    }
}
