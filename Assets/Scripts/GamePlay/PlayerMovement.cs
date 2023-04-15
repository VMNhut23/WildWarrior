using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpHeight;

    private float dirX;
    private bool isDoubleJump;
    private enum MovementState { Idle, Running, Jumping, Falling };
    private MovementState movementState;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        Jumping();
        UpdateAnimations();
        Moving();
    }
    private void Jumping()
    {
        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            isDoubleJump = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded() || isDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                isDoubleJump = !isDoubleJump;
                animator.SetBool("DoubleJump", !isDoubleJump);
            }
        }
    }
    private void Moving()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
        {
            rb.velocity = new Vector2(dirX * playerSpeed, rb.velocity.y);
        }
    }
    private void UpdateAnimations()
    {
        if (dirX > 0f)
        {
            spriteRenderer.flipX = false;
            movementState = MovementState.Running;
        }
        else if (dirX < 0f)
        {
            spriteRenderer.flipX = true;
            movementState = MovementState.Running;
        }
        else
        {
            movementState = MovementState.Idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            movementState = MovementState.Jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            movementState = MovementState.Falling;
        }

        animator.SetInteger("State", (int)movementState);
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }
}
