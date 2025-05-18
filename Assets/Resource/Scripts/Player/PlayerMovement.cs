using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;
    private bool canMove = true;

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        movement = Vector2.zero;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}
