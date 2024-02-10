using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpForce;
    public float fatigueTimer = 5.0f; // Set duration before player gets tired (5 seconds in this example)

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Coroutine sitCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleHorizontalMovement();
        PerformJump();

        if (!animator.GetBool("isMoving") && sitCoroutine == null) {
            sitCoroutine = StartCoroutine(SitAfterWait());
        }
    }

    IEnumerator SitAfterWait()
    {
        yield return new WaitForSecondsRealtime(fatigueTimer);
        Sit();
    }

    private void HandleHorizontalMovement()
    {
         // Horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);
        animator.SetBool("isMoving", rb.velocity.x != 0);

       if (shouldFlipSprite(horizontalInput,spriteRenderer.flipX)) 
       {
            spriteRenderer.flipX = !spriteRenderer.flipX;
       }

        if (rb.velocity.x != 0 && sitCoroutine != null) {
            StopCoroutine(sitCoroutine);
            sitCoroutine = null; 
        }
    }

    private void PerformJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            StopCoroutine(sitCoroutine);
            sitCoroutine = null; 
        }

    }

    private void Sit()
    {
        animator.SetTrigger("isSitting");
    }

    // Check if player is grounded (optional)
    private bool IsGrounded()
    {
        Vector2 raycastOrigin = transform.position;
        float raycastLength = 0.1f; // Adjust this based on your player collider size
        int groundLayer = LayerMask.GetMask("Ground"); // Assuming your layer is named "Ground"

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, raycastLength, groundLayer);
        // Now `hit` will only contain collisions with objects in the "Ground" layer
        return hit.collider != null;
    }

    private bool shouldFlipSprite(float horizontalInput, bool flipX)
    {
        return horizontalInput < 0 && !flipX || horizontalInput > 0 && flipX;
    }
}