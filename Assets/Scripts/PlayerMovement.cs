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
    private bool isMoving;

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
            HandleSittingIdleState();
            break;
            case PlayerState.StandingUp:
            HandleStandingUpState();
            break;
            case PlayerState.SittingDown:
            HandleSittingDownState();
            break;
            case PlayerState.Walking:
            HandleWalkingState();
            break;
        }
    }

    IEnumerator SitAfterWait()
    {
        yield return new WaitForSecondsRealtime(fatigueTimer);
        currentState = PlayerState.SittingDown;
    }

    private void HandleStandingIdleState()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        isMoving = horizontalInput != 0;
        animator.SetBool("isMoving", isMoving);
        if (isMoving) { currentState = PlayerState.Walking; }
        if (!isMoving && sitCoroutine == null) { sitCoroutine = StartCoroutine(SitAfterWait()); }
    }
    private void HandleSittingIdleState() {
        float horizontalInput = Input.GetAxis("Horizontal");
        isMoving = horizontalInput != 0;
        if (isMoving) { currentState = PlayerState.StandingUp; }
    }

    private void HandleSittingDownState()
    {
        isMoving = false;
        animator.SetBool("isSat", true);
        animator.SetBool("isMoving", false);
        animator.SetTrigger("sit");
        currentState = PlayerState.SittingIdle;
    }

    private void HandleStandingUpState()
    {
        animator.SetTrigger("standup");
        animator.SetBool("isSat", false);
        currentState = PlayerState.StandingIdle;
    }

    private void HandleWalkingState() 
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("StandingIdle") || stateInfo.IsName("Walking"))
        {
            // Player is not in sitting state, allow walking
            float horizontalInput = Input.GetAxis("Horizontal");
            isMoving = Mathf.Abs(horizontalInput) > 0.1f;
            if (isMoving) {
                animator.SetBool("isMoving", true);
                rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);

                if (sitCoroutine != null) 
                {
                    StopCoroutine(sitCoroutine);
                    sitCoroutine = null; 
                }

                if (shouldFlipSprite(horizontalInput,spriteRenderer.flipX)) 
                {
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                }
            }
        }
        else
        {
            // Player is in sitting state, prevent walking
            // You may choose to disable movement or take other actions here
        }
            if (!isMoving) { currentState = PlayerState.StandingIdle; }
        }
            private bool shouldFlipSprite(float horizontalInput, bool flipX)
        {
            return horizontalInput < 0 && !flipX || horizontalInput > 0 && flipX;
        }
    }