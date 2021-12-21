using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

// This component executes solely on the player owned by the local client, and performs client-side prediction.
public class PlayerMovementPrediction : MonoBehaviour
{
    private PlayerInput input;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;
    private new Collider collider;

    private Transform head;

    private float clientCurrentVerticalCameraRotation;

    private bool canMoveCamera = true;

    private float jumpCooldown = 1.5f;
    private float currentJumpCooldown;

    private bool onGround;

    private float coyoteTime = 0.2f;
    private float currentCoyoteTime;

    // How far the player's server position can be from local position before reconciliating
    [SerializeField]
    private float reconciliationThreshold = 1f;

    // The most recent server state recieved
    private PlayerStatePacket lastServerState;
    public void UpdateServerState(PlayerStatePacket s) => lastServerState = s;

    // Start is called before the first frame update
    void Start()
    {
        input = transform.parent.GetComponent<PlayerInput>();
        settings = GetComponent<PlayerSettings>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        collider = transform.Find("Model").GetComponent<Collider>();
        head = transform.Find("Head");
    }

    void Update()
    {
        // Rotation is updated every frame locally for responsiveness
        PredictRotationalMovement();

        if (Vector3.Distance(lastServerState.position, transform.position) >= reconciliationThreshold)
        {
            rb.MovePosition(lastServerState.position);
        }
    }

    void FixedUpdate()
    {
        PredictGroundCheck();
        PredictDirectionalMovement();
        UpdateJump();
    }

    private void PredictRotationalMovement()
    {
        // Get mouse input from PlayerInput component
        // Multiply it by sensitivity
        Vector2 mouseInput = input.GetMouseInputVector() * settings.Sensitivity;

        Vector3 horizontalRotation = new Vector3(0f, mouseInput.x, 0f);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(horizontalRotation));
        if (head != null)
        {
            clientCurrentVerticalCameraRotation -= mouseInput.y;
            Vector3 verticalRotation = new Vector3(clientCurrentVerticalCameraRotation, 0f, 0f);

            head.transform.localEulerAngles = verticalRotation;
        }
    }

    private void PredictDirectionalMovement()
    {
        // Get client input from the PlayerInput component
        // Normalise it so that diagonal movement isn't faster than cardinal movement
        Vector3 moveInput = input.GetWalkInputVector().normalized;
        // Apply move speed modifiers
        moveInput.x *= stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier;
        moveInput.z *= moveInput.z > 0f ? (input.sprinting ? (stats.MoveSprintSpeed * stats.MoveSprintMultiplier) : (stats.MoveWalkSpeed * stats.MoveWalkMultiplier)) : (stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier);
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.z;

        // Don't update the rigidbody if we aren't actually going to move
        if (movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
    }

    private void PredictGroundCheck()
    {
        // How far below the hitbox the ground is measured
        float groundCheckDistance = 0.05f;
        // Check for environment colliders in a small area below our hitbox
        Vector3 p = new Vector3(collider.bounds.center.x, collider.bounds.center.y - collider.bounds.extents.y - (groundCheckDistance / 2f), collider.bounds.center.z);
        bool newOnGround = Physics.OverlapSphere(p, groundCheckDistance / 2f).Where(x => x.tag == "Environment").Count() > 0;

        // If we were on the ground last frame, and not anymore, begin coyote time countdown
        if (onGround && !newOnGround)
        {
            currentCoyoteTime = coyoteTime;
        }
        // Decrement coyote time until 0
        if (currentCoyoteTime >= 0)
        {
            currentCoyoteTime -= Time.fixedDeltaTime;
        }
        // Update onGround
        onGround = newOnGround;
    }

    private void UpdateJump()
    {
        // Decrement jump cooldown
        if (currentJumpCooldown > 0)
        {
            currentJumpCooldown -= Time.fixedDeltaTime;
        }
        // Standard jump check
        if (onGround && input.GetJumpKeyPressed() && currentJumpCooldown <= 0)
        {
            rb.AddForce(new Vector3(0f, stats.JumpForce, 0f), ForceMode.Impulse);
            currentJumpCooldown = jumpCooldown;
        }
        // Coyote jump check
        else if (currentCoyoteTime > 0 && input.GetJumpKeyPressed() && currentJumpCooldown <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, stats.JumpForce, rb.velocity.z);
            currentJumpCooldown = jumpCooldown;
            currentCoyoteTime = 0f;
        }
    }
}
