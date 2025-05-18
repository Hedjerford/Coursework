using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float followDistance = 2f;
    private NavMeshAgent agent;
    private bool shouldFollow = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (shouldFollow && Vector3.Distance(transform.position, player.position) > followDistance)
        {
            agent.SetDestination(player.position);
        }
    }

    public void StartFollowing()
    {
        shouldFollow = true;
    }

    public void StopFollowing()
    {
        shouldFollow = false;
        agent.ResetPath();
    }
}
