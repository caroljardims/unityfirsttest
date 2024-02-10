using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        StandingIdle,
        SittingIdle,
        Walking,
        StandingUp,
        SittingDown,
    }
    public float MoveSpeed;
    public float JumpForce;
    public float fatigueTimer = 5.0f; // Set duration before player gets tired (5 seconds in this example)

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerState currentState;
    private Coroutine sitCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentState = PlayerState.StandingIdle;
    }

    void Update()
    {

        switch (currentState){
            case PlayerState.StandingIdle:
            HandleStandingIdleState();
            break;
            case PlayerState.SittingIdle:
            break;
            case PlayerState.StandingUp:
            break;
            case PlayerState.SittingDown:
            break;
            case PlayerState.Walking:
            HandleWalkingState();
            break;
        }

        if (!animator.GetBool("isMoving") && sitCoroutine == null) {
            sitCoroutine = StartCoroutine(SitAfterWait());
        }
    }

    IEnumerator SitAfterWait()
    {
        yield return new WaitForSecondsRealtime(fatigueTimer);
        Sit();
    }

    private void HandleStandingIdleState()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isMoving = horizontalInput != 0;
        if (isMoving) { currentState = PlayerState.Walking; }
        if (!isMoving && sitCoroutine == null) { sitCoroutine = StartCoroutine(SitAfterWait()); }
    }

    private void HandleWalkingState() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isMoving = horizontalInput != 0;
        animator.SetBool("isMoving", isMoving);
        rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);
        
        if (sitCoroutine != null) {
            StopCoroutine(sitCoroutine);
            sitCoroutine = null; 
        }

       if (shouldFlipSprite(horizontalInput,spriteRenderer.flipX)) 
       {
            spriteRenderer.flipX = !spriteRenderer.flipX;
       }
        if (horizontalInput == 0) { currentState = PlayerState.StandingIdle; }
    }

    private void Sit()
    {
        animator.SetBool("isSat", true);
        animator.SetBool("isMoving", false);
        animator.SetTrigger("sit");
    }

    private void StandUp()
    {
        animator.SetBool("isSat", false);
        animator.SetBool("isMoving", true);
        animator.SetTrigger("standup");
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

    // private void PerformJump()
    // {
    //     if (Input.GetButtonDown("Jump") && IsGrounded())
    //     {
    //         rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    //         StopCoroutine(sitCoroutine);
    //         sitCoroutine = null; 
    //     }

    // }
}