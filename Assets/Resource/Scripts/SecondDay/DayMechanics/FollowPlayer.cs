using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float followDistance = 2f;
    private NavMeshAgent agent;
    private bool shouldFollow = false;

    void Awake()
    {
        // ����� �������� � Z �������, ����� NPC ��� �����
        transform.rotation = Quaternion.identity;
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ���������� Z = 0
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        if (shouldFollow && Vector3.Distance(transform.position, player.position) > followDistance)
        {
            Vector3 target = player.position;
            target.z = 0f; // ����� ����� �� ��������� ������
            agent.SetDestination(target);
        }
    }

    public void StartFollowing()
    {
        shouldFollow = true;
    }

    public void StopFollowing()
    {
        shouldFollow = false;
        if (agent != null)
            agent.ResetPath();
    }
}
