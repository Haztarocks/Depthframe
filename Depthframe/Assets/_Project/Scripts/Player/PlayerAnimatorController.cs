using UnityEngine;
using DG.Tweening;
using CharacterCreator2D;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterViewer characterViewer;
    [SerializeField] private Rigidbody2D rb;
    
    [Header("Animation Settings")]
    [SerializeField] private float runAnimationSpeed = 1f;
    [SerializeField] private float jumpSquashAmount = 0.2f;
    [SerializeField] private float jumpSquashDuration = 0.2f;
    
    private bool isGrounded;
    private bool isCrouching;
    private Vector2 originalScale;
    private Sequence jumpSequence;
    private Animator animator;
    private float currentScaleY;

    private void Start()
    {
        if (characterViewer == null)
        {
            characterViewer = GetComponent<CharacterViewer>();
        }
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        currentScaleY = originalScale.y; // Add this line
    }

    private void Update()
    {
        // Handle movement animations
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Handle crouch
        bool crouchInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        HandleCrouch(crouchInput);
        
        if (Mathf.Abs(horizontalInput) > 0.1f && !isCrouching)
        {
            // Set walk animation parameter
            animator.SetBool("Walk", true);
            animator.speed = runAnimationSpeed;
            
            // Flip character based on direction
            transform.localScale = new Vector3(
                horizontalInput < 0 ? -Mathf.Abs(originalScale.x) : Mathf.Abs(originalScale.x),
                originalScale.y,
                1
            );
        }
        else
        {
            // Return to idle
            animator.SetBool("Walk", false);
            animator.speed = 1f;
        }
        
        // Handle jump animation with debug
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump button pressed");
                Debug.Log("Player is grounded");
                if (!isCrouching)
                {
                    Debug.Log("Player can jump (not crouching)");
                    PlayJumpAnimation();
                }
        }
        
        // Handle fall animation
        if (!isGrounded)
        {
            animator.SetBool("Fall", rb.linearVelocity.y < -0.1f);
        }
    }

    private void HandleCrouch(bool shouldCrouch)
    {
        if (shouldCrouch != isCrouching)
        {
            isCrouching = shouldCrouch;
            animator.SetBool("Crouch", isCrouching);
        }
    }

    private void PlayJumpAnimation()
    {
        Debug.Log("PlayJumpAnimation called");
        
        // Kill any existing jump sequence
        jumpSequence?.Kill();
        
        // Create new jump sequence
        jumpSequence = DOTween.Sequence();
        
        // Trigger jump animation BEFORE squash effect
        animator.SetTrigger("Jump");
        Debug.Log("Jump trigger set");
        
        // Squash effect after setting the trigger
        jumpSequence.Append(transform.DOScaleY(originalScale.y - jumpSquashAmount, jumpSquashDuration * 0.5f));
        jumpSequence.Append(transform.DOScaleY(originalScale.y, jumpSquashDuration * 0.5f));
    }

    public void PlayLandingAnimation()
    {
        // Play landing squash effect
        transform.DOScaleY(originalScale.y - jumpSquashAmount * 0.5f, jumpSquashDuration * 0.5f)
            .OnComplete(() => transform.DOScaleY(originalScale.y, jumpSquashDuration * 0.5f));
            
        // Return to idle or walk based on movement
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

    // Call this from your ground check system
    public void SetGrounded(bool grounded)
    {
        if (!isGrounded && grounded)
        {
            PlayLandingAnimation();
            animator.SetBool("Fall", false);
        }
        isGrounded = grounded;
    }
}