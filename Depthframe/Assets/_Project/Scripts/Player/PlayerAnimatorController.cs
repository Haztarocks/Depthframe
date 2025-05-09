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
    
    private bool isGrounded;
    private bool isCrouching;
    private Vector2 originalScale;
    private Animator animator;

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
    }

    private void Update()
    {
        // Handle movement animations
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Handle crouch toggle
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            animator.SetBool("Crouch", isCrouching); // Use the existing 'Crouch' parameter
        }
        
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
            if (!isCrouching)
            {
                PlayJumpAnimation();
            }
        }
        
        // Handle fall animation
        if (!isGrounded)
        {
            animator.SetBool("Fall", rb.linearVelocity.y < -0.1f);
        }
    }

    private void PlayJumpAnimation()
    {

        
        // Simply trigger the jump animation
        animator.SetTrigger("Jump");

    }

    public void PlayLandingAnimation()
    {
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