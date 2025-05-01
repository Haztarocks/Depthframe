using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal; // Needed for Light2D

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float crouchSpeedMultiplier = 0.5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float groundCheckDistance = 0.1f;

    [Header("Components")]
    public LayerMask groundLayer;
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    [Header("Torch")]
    public Light2D torchLight; // Assign your Light2D in the Inspector
    private bool torchOn = false;
    private TorchBattery torchBattery; // Reference to the new battery script

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isCrouching;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        torchBattery = GetComponent<TorchBattery>(); // Get the TorchBattery component
        torchOn = false;
        if (torchLight != null)
            torchLight.enabled = false;
    }

    // Add these methods for the Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            if (standingCollider != null) standingCollider.enabled = false;
            if (crouchingCollider != null) crouchingCollider.enabled = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
            if (standingCollider != null) standingCollider.enabled = true;
            if (crouchingCollider != null) crouchingCollider.enabled = false;
        }
    }

    public void OnToggleTorch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            torchOn = !torchOn;
            if (torchLight != null && torchBattery != null)
                torchLight.enabled = torchOn && torchBattery.HasBattery();
        }
    }

    private void Update()
    {
        // Ground check using player's collider
        Bounds bounds = playerCollider.bounds;
        isGrounded = Physics2D.BoxCast(
            bounds.center, 
            bounds.size, 
            0f, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayer
        );

        // Coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jump if conditions are met
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }

        // Torch logic
        if (torchOn && torchBattery != null)
        {
            if (!torchBattery.DrainBattery(Time.deltaTime))
            {
                torchOn = false;
                if (torchLight != null)
                    torchLight.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Movement with crouch
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);
    }
}