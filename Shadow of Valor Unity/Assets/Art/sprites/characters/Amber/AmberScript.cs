using UnityEngine;

public class AmberScript : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D amberRigidbody;
    public Animator animator;                 // add this

    [Header("Jump Settings")]
    public float jumpStrength = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Feel Tweaks")]
    public float coyoteTime = 0.10f;
    public float jumpBufferTime = 0.10f;

    private bool grounded;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private int jumpsUsed;

    private void Update()
    {
        // Jump input buffer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            if (jumpBufferTimer > 0f)
            {
                jumpBufferTimer -= Time.deltaTime;
            }
        }

        updateGrounded();

        // Animation hook: set bool based on ground
        animator.SetBool("isJumping", !grounded);

        // Use buffered jump
        if (jumpBufferTimer > 0f)
        {
            bool canGroundJump = grounded || coyoteTimer > 0f;
            bool canAirJump = jumpsUsed < 1;

            if (canGroundJump || canAirJump)
            {
                performJump(canGroundJump);
                jumpBufferTimer = 0f;
            }
        }
    }

    private void updateGrounded()
    {
        bool wasGrounded = grounded;

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (grounded)
        {
            coyoteTimer = coyoteTime;
            if (!wasGrounded) jumpsUsed = 0;
        }
        else
        {
            if (coyoteTimer > 0f) coyoteTimer -= Time.deltaTime;
        }
    }

    private void performJump(bool usedGroundJump)
    {
        Vector2 v = amberRigidbody.linearVelocity;
        v.y = jumpStrength;
        amberRigidbody.linearVelocity = v;

        if (!usedGroundJump)
        {
            jumpsUsed += 1;
        }

        coyoteTimer = 0f;
    }
}

