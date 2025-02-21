using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyv2 : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGravityFlipped = false;
    private float flipCooldown = 10f; // Time in seconds between gravity flips
    private float jumpCooldown = 2f; // Time in seconds between random jumps
    private float nextFlipTime;
    private float nextJumpTime;
    private Collider2D enemyCollider;

    public float speed = 5f;
    public float jumpForce = 15f;

    [SerializeField] private LayerMask groundLayer;
    private SpriteRenderer spriteRenderer;
    private float horizontalDirection; // 1 for right, -1 for left

    private float stopTime; // Time to stop moving
    private bool isStopped = false; // Whether the enemy is stopped
    private float nextStopTime;
    private float nextDirectionChangeTime; // Time to check for a new random direction

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        // Randomly choose the initial horizontal direction (1 = right, -1 = left)
        horizontalDirection = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;

        // Flip the sprite if the direction is right
        if (horizontalDirection == 1f)
        {
            spriteRenderer.flipX = true;
        }

        nextFlipTime = Time.time + Random.Range(1f, flipCooldown);
        nextJumpTime = Time.time + Random.Range(1f, jumpCooldown);
        nextDirectionChangeTime = Time.time + Random.Range(2f, 5f); // Wait between 2 and 5 seconds for the first direction change
        nextStopTime = Time.time + Random.Range(3f, 7f);
    }

    private void Update()
    {
        // If the enemy is not stopped, move horizontally and possibly reverse direction
        if (!isStopped)
        {
            MoveHorizontally();
        }

        // Randomly stop moving
        if (Time.time >= nextStopTime && !isStopped) // 1% chance to stop moving
        {
            StopMovingForRandomTime();
            nextStopTime = Time.time + Random.Range(3f, 7f);
        }

        // Randomly change direction every few seconds (between 2 and 5 seconds)
        if (Time.time >= nextDirectionChangeTime && !isStopped) // Only change direction if not stopped
        {
            ReverseDirection();
            nextDirectionChangeTime = Time.time + Random.Range(2f, 5f); // Randomize the next time the direction will change
        }

        // Check for gravity flip
        if (Time.time >= nextFlipTime && IsGrounded())
        {
            FlipGravity();
            nextFlipTime = Time.time + Random.Range(1f, flipCooldown);
        }

        // Check for random jump
        if (Time.time >= nextJumpTime && IsGrounded())
        {
            Jump();
            nextJumpTime = Time.time + Random.Range(1f, jumpCooldown);
        }
    }

    private void MoveHorizontally()
    {
        // Move the enemy horizontally, keeping the direction even when gravity flips
        rb.velocity = new Vector2(horizontalDirection * speed, rb.velocity.y);
    }

    private void FlipGravity()
    {
        // Flip the gravity direction
        rb.gravityScale *= -1;
        isGravityFlipped = !isGravityFlipped;

        // Flip the sprite vertically
        spriteRenderer.flipY = isGravityFlipped;

        // Ensure proper alignment after flipping
        transform.rotation = Quaternion.identity;
    }

    private void Jump()
    {
        // Apply jump force in the current gravity direction
        Vector2 jumpDirection = isGravityFlipped ? Vector2.down : Vector2.up;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpDirection.y);
    }

    private bool IsGrounded()
    {
        // Check if the enemy is on the ground (or ceiling when gravity is flipped)
        Vector2 groundCheckOrigin = enemyCollider.bounds.center;
        Vector2 groundCheckSize = enemyCollider.bounds.size;
        Vector2 groundCheckDirection = isGravityFlipped ? Vector2.up : Vector2.down;

        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheckOrigin,
            groundCheckSize,
            0f,
            groundCheckDirection,
            0.1f,
            groundLayer
        );

        return hit.collider != null;
    }

    private void StopMovingForRandomTime()
    {
        // Randomly stop the movement for a time between 3 and 7 seconds
        stopTime = Random.Range(3f, 7f);
        isStopped = true;
        StartCoroutine(ResumeMovementAfterTime());
    }

    private IEnumerator ResumeMovementAfterTime()
    {
        // Wait for the stop time and then resume movement
        yield return new WaitForSeconds(stopTime);
        isStopped = false;
    }

    private void ReverseDirection()
    {
        // Reverse the horizontal direction
        horizontalDirection *= -1;

        // Flip the sprite if direction is reversed
        spriteRenderer.flipX = !spriteRenderer.flipX;
        
        nextStopTime = Time.time + Random.Range(3f, 7f);
    }
}
