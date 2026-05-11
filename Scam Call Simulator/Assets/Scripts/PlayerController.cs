using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top-Down Player Controller
/// Attach this script to your player GameObject.
///
/// Requirements:
///   - A Rigidbody2D component on the same GameObject
///   - (Optional) An Animator component for movement animations
///
/// Controls: WASD or Arrow Keys
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units per second")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Optional: Animator")]
    [Tooltip("Assign if you have an Animator with 'MoveX', 'MoveY', and 'IsMoving' parameters")]
    [SerializeField] private Animator animator;

    // Private references
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection; // Remembers last direction for idle facing

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Freeze rotation so the player doesn't tip over from physics
        rb.freezeRotation = true;

        // If no animator was assigned in Inspector, try to find one
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GatherInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// Read WASD / Arrow Key input each frame.
    /// </summary>
    private void GatherInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        // Normalize so diagonal movement isn't faster
        moveInput = new Vector2(horizontal, vertical).normalized;

        // Track the last non-zero direction for idle-facing
        if (moveInput != Vector2.zero)
            lastMoveDirection = moveInput;
    }

    /// <summary>
    /// Apply velocity via Rigidbody2D (physics-based, collision-friendly).
    /// </summary>
    private void MovePlayer()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Send movement data to the Animator (if one is assigned).
    /// Expects these Animator parameters:
    ///   - Float  "MoveX"    — horizontal direction
    ///   - Float  "MoveY"    — vertical direction
    ///   - Bool   "IsMoving" — true while the player is moving
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("IsMoving", isMoving);

        // Use last direction when idle so the character faces the way they stopped
        Vector2 animDirection = isMoving ? moveInput : lastMoveDirection;
        animator.SetFloat("MoveX", animDirection.x);
        animator.SetFloat("MoveY", animDirection.y);
    }

    // -----------------------------------------------------------------------
    // Public API — call these from other scripts if needed
    // -----------------------------------------------------------------------

    /// <summary>Returns the player's current speed setting.</summary>
    public float GetMoveSpeed() => moveSpeed;

    /// <summary>Change the move speed at runtime (e.g. speed power-up).</summary>
    public void SetMoveSpeed(float newSpeed) => moveSpeed = Mathf.Max(0f, newSpeed);

    /// <summary>Returns true while the player is providing movement input.</summary>
    public bool IsMoving() => moveInput != Vector2.zero;

    public void FreezePlayer()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
    }
}

