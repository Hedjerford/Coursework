using UnityEngine;

public class BoatEnterTrigger : MonoBehaviour
{
    public Transform boatSeat;
    public float enterRadius = 2f;
    public string hintMessage = "Нажмите E, чтобы сесть в катер";

    public BoatController boatController; // Скрипт управления катером
    public GameObject playerObject;       // Игрок как объект (для SetActive)

    private Transform player;
    private PlayerMovement playerMovement;

    private bool isPlayerNear = false;
    private bool playerInBoat = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerObject ??= player?.gameObject;
        playerMovement = player?.GetComponent<PlayerMovement>();

        if (boatController != null)
            boatController.enabled = false;
    }

    void Update()
    {
        if (player == null || playerInBoat) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNear = distance <= enterRadius;

        if (isPlayerNear)
        {
            InteractionHintController.Instance.hintText.text = hintMessage;
            InteractionHintController.Instance.ShowHint(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterBoat();
                FindObjectOfType<SmoothFollow>().SetTarget(boatController.transform);
            }
        }
        else
        {
            InteractionHintController.Instance.ShowHint(false);
        }
    }

    void EnterBoat()
    {
        Debug.Log("🛥 Игрок сел в катер");
        playerInBoat = true;

        InteractionHintController.Instance.ShowHint(false);

        if (playerMovement != null)
            playerMovement.DisableMovement();

        if (boatSeat != null)
            player.position = boatSeat.position;

        if (playerObject != null)
            playerObject.SetActive(false); // ⛔️ отключаем игрока полностью

        if (boatController != null)
            boatController.enabled = true; // ✅ передаём управление катеру
    }
}
