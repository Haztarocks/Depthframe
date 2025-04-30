using UnityEngine;
using AC;

public class Player2DPlatformerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float jumpForce = 6f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Jump Settings")]
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 2.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Player player;

    private bool isGrounded;
    private bool isCrouching;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        // Remove direct animator reference since we'll use AC's animation system
    }

    private void Update()
    {
        if (KickStarter.stateHandler.gameState != GameState.Normal) return;

        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        
        // Update vertical velocity through animator directly
        if (player != null && player.GetAnimator() != null)
        {
            player.GetAnimator().SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        
        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);
        
        if (player != null && player.GetAnimator() != null)
        {
            player.GetAnimator().SetFloat("Speed", Mathf.Abs(moveX));
        }

        // Remove the manual flipping code since AC will handle it
    }

    private void HandleGroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (wasGrounded && !isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else if (isGrounded)
        {
            coyoteTimeCounter = 0;
            isJumping = false;
        }
        
        // Use AC's animation system
        if (player != null && player.GetAnimator() != null)
        {
            player.GetAnimator().SetBool("IsGrounded", isGrounded);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && (isGrounded || coyoteTimeCounter > 0f) && !isCrouching)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            
            if (player != null && player.GetAnimator() != null)
            {
                player.GetAnimator().SetTrigger("Jump");
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            isCrouching = true;
            if (player != null && player.GetAnimator() != null)
            {
                player.GetAnimator().SetBool("IsCrouching", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            isCrouching = false;
            if (player != null && player.GetAnimator() != null)
            {
                player.GetAnimator().SetBool("IsCrouching", false);
            }
        }
    }
}
