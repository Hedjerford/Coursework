using UnityEngine;

public class BirdFeeder : MonoBehaviour
{
    private bool playerInRange = false;
    private bool hasBeenFed = false;

    public Animator animator;
    private void Update()
    {
        if (playerInRange && !hasBeenFed && Input.GetKeyDown(KeyCode.E))
        {
            hasBeenFed = true;
            FindObjectOfType<BirdCounter>().FeedBird();
            animator.SetBool("Eating", true);


        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;

        if (!hasBeenFed)
            InteractionHintController.Instance.ShowHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionHintController.Instance.ShowHint(false);
            playerInRange = false;
        }   
    }
}
