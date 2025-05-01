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

    private Animator animator; // Add this line

private void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    playerCollider = GetComponent<Collider2D>();
    torchBattery = GetComponent<TorchBattery>(); // Get the TorchBattery component
    torchOn = false;
    if (torchLight != null)
    {
        torchLight.enabled = false;
    }
    animator = GetComponent<Animator>(); // Add this line
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
            Debug.Log("Toggle Torch called");
            torchOn = !torchOn;
            Debug.Log("Torch On: " + torchOn);
            if (torchLight != null && torchBattery != null)
            {
                bool hasBattery = torchBattery.HasBattery();
                Debug.Log("Has Battery: " + hasBattery);
                torchLight.enabled = torchOn && hasBattery;
            }
        }
    }

    private bool isAiming = false;

    // Add this method for the Input System
    public void OnAimMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
            if (animator != null)
                animator.SetBool("AimMode", true);
        }
        else if (context.canceled)
        {
            isAiming = false;
            if (animator != null)
                animator.SetBool("AimMode", false);
        }
    }

    private bool hasFired = false;
    public float fireRate = 0.25f; // Time in seconds between shots
    private float fireCooldown = 0f;

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

        // Only update aim direction if aiming
        if (isAiming)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z); // Ensure correct z for 2D
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Vector2 toMouse = mouseWorldPos - transform.position;
            float verticalAim = toMouse.y;

            // If the mouse is close to the player's y-position, aim front (0)
            if (Mathf.Abs(verticalAim) < 0.5f)
                verticalAim = 0f;
            else
                verticalAim = Mathf.Clamp(verticalAim, -1f, 1f);

            if (animator != null)
                animator.SetFloat("Aim", verticalAim);
        }

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

        // Fire cooldown timer
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Movement with crouch
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);
    }

    private void UpdateAim(float aimValue)
    {
        if (animator != null)
            animator.SetFloat("Aim", aimValue);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        // Only allow firing in AimMode
        if (!isAiming)
            return;

        // Only fire if cooldown is zero and input is pressed
        if (context.started && !hasFired && fireCooldown <= 0f)
        {
            hasFired = true;
            fireCooldown = fireRate;
            if (animator != null)
                animator.SetTrigger("Fire"); // Make sure you have a "Fire" trigger in your Animator
            // Add your firing logic here (e.g., instantiate projectile)
        }
        else if (context.canceled)
        {
            hasFired = false;
        }
    }
}