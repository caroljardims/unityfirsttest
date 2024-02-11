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
        LeaningDown,
        Eating,
        LeaningUp,
    }
    public float MoveSpeed;
    public float JumpForce;
    public float fatigueTimer = 5.0f; // Set duration before player gets tired (5 seconds in this example)

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerState currentState;
    private Coroutine sitCoroutine;
    private bool isWalking;
    private bool isEating;

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
            case PlayerState.LeaningDown:
            HandleLeaningDownState();
            break;
            case PlayerState.Eating:
            HandleEatingState();
            break;
            case PlayerState.LeaningUp:
            HandleLeaningUpState();
            break;
        }

        HandleCoroutine();
    }

    IEnumerator SitAfterWait()
    {
        yield return new WaitForSecondsRealtime(fatigueTimer);
        currentState = PlayerState.SittingDown;
    }

    private void HandleCoroutine() {
        if (currentState == PlayerState.StandingIdle && !isWalking && sitCoroutine == null) 
        {
            sitCoroutine = StartCoroutine(SitAfterWait());
        }
        if (currentState != PlayerState.StandingIdle && isWalking && sitCoroutine != null)
        {
            StopCoroutine(sitCoroutine);
            sitCoroutine = null;
        }
    }
    private void HandleStandingIdleState()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        isWalking = horizontalInput != 0;
        animator.SetBool("isMoving", isWalking);
        if (isWalking) { currentState = PlayerState.Walking; }
        if (!isWalking && verticalInput < 0) 
        { 
            animator.SetTrigger("eat");
            currentState = PlayerState.LeaningDown;
        }
    }
    private void HandleSittingIdleState() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        isWalking = horizontalInput != 0;
        if (isWalking || verticalInput > 0) { currentState = PlayerState.StandingUp; }
    }

    private void HandleSittingDownState()
    {
        isWalking = false;
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
            isWalking = Mathf.Abs(horizontalInput) > 0.1f;
            if (isWalking) {
                animator.SetBool("isMoving", true);
                rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);

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
            if (!isWalking) { currentState = PlayerState.StandingIdle; }
    }
    
    private void HandleLeaningDownState()
    {
        animator.SetBool("isEating", true);
        currentState = PlayerState.Eating;
    }

    private void HandleEatingState()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (verticalInput > 0 || horizontalInput != 0) {
            animator.SetBool("isEating", false);
            currentState = PlayerState.LeaningUp;
        }
    }

    private void HandleLeaningUpState()
    {
        currentState = PlayerState.StandingUp;
    }
    private bool shouldFlipSprite(float horizontalInput, bool flipX)
    {
        return horizontalInput < 0 && !flipX || horizontalInput > 0 && flipX;
    }
}