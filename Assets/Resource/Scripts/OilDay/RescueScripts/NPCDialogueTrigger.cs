using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public float triggerRadius = 2f;
    [TextArea(3, 10)] public string[] dialogueLines;

    private bool hasSpoken = false;
    private EmergencyDialogue dialogueUI;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        dialogueUI = FindObjectOfType<EmergencyDialogue>();
    }

    void Update()
    {
        if (hasSpoken || player == null || dialogueUI == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < triggerRadius)
        {
            hasSpoken = true;
            dialogueUI.StartDialogueLines(dialogueLines, OnDialogueFinished);
        }

        LockZ();
    }

    void OnDialogueFinished()
    {
        
        Debug.Log("📢 Диалог с НПС завершён");

        BoomPlacementController.Instance?.StartBoomMission();
    }

    void LockZ()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
