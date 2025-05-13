using UnityEngine;

public class TrashItem : MonoBehaviour
{
    private bool playerInRange = false;
    private bool collected = false;

    private void Update()
    {
        if (playerInRange && !collected && Input.GetKeyDown(KeyCode.E))
        {
            collected = true;
            FindObjectOfType<TrashCounter>()?.CollectTrash();
            InteractionHintController.Instance.ShowHint(false);  // убрать подсказку
            Destroy(gameObject); // или отключить: gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            playerInRange = true;
            InteractionHintController.Instance.ShowHint(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionHintController.Instance.ShowHint(false);
        }
    }
}
