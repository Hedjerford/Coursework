using UnityEngine;
using UnityEngine.AI;

public class NPCFollower : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void FollowPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(Follow());
    }

    private System.Collections.IEnumerator Follow()
    {
        while (true)
        {
            if (player != null)
                agent.SetDestination(player.position);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
