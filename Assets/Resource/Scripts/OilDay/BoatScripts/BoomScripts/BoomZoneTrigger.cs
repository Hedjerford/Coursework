using UnityEngine;

public class BoomZoneTrigger : MonoBehaviour
{
    public static BoomZoneTrigger Instance { get; private set; }

    private bool playerInside = false;
    private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boat"))
        {
            playerInside = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boat"))
        {
            playerInside = false;
        }
    }

    public bool IsPlayerInside() => playerInside;
    public Vector3 GetPlayerPosition() => playerInside ? player.position : Vector3.zero;
    public Vector3 GetZoneCenter() => transform.position;
}
