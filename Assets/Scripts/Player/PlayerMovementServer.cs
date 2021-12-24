using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerMovementServer : NetworkBehaviour
{
    #region component references
    private PlayerSynchroniser sync;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;
    private Collider col;
    private NetworkIdentity id;

    private Transform head;
    #endregion

    private float cameraRotationLimitUp = 85f;
    private float cameraRotationLimitDown = -90f;

    private bool canMoveCamera = true;

    private float jumpCooldown = 1.5f;
    private float currentJumpCooldown;

    private bool onGround;

    private float coyoteTime = 0.2f;
    private float currentCoyoteTime;

    // Start is called before the first frame update
    void Start()
    {
        sync = transform.parent.GetComponent<PlayerSynchroniser>();
        id = transform.parent.GetComponent<NetworkIdentity>();
        settings = GetComponent<PlayerSettings>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        col = transform.Find("Model").GetComponent<Collider>();
        head = transform.Find("Head");
    }

    void FixedUpdate()
    {
        if (!id.isServer)
            return;

        UpdateGroundCheck();
        UpdateDirectionalMovement();
        UpdateJump();
    }

    public void RecieveRotationalMovement(Vector3 newRot)
    {
        if (!canMoveCamera)
            return;
        Vector3 horizontalRotation = new Vector3(0f, newRot.x, 0f);
        rb.MoveRotation(Quaternion.Euler(horizontalRotation));
        if (head != null)
        {
            float vertRot =  newRot.y;
            vertRot = Mathf.Clamp(vertRot, cameraRotationLimitDown, cameraRotationLimitUp);
            Vector3 verticalRotation = new Vector3(vertRot, 0f, 0f);

            head.transform.localEulerAngles = verticalRotation;
        }
    }

    private void UpdateJump()
    {
        // Decrement jump cooldown
        if (currentJumpCooldown > 0)
        {
            currentJumpCooldown -= Time.fixedDeltaTime;
        }
        // Standard jump check
        if (onGround && sync.InputPacket.jumpInput && currentJumpCooldown <= 0)
        {
            rb.AddForce(new Vector3(0f, stats.JumpForce, 0f), ForceMode.Impulse);
            currentJumpCooldown = jumpCooldown;
        }
        // Coyote jump check
        else if (currentCoyoteTime > 0 && sync.InputPacket.jumpInput && currentJumpCooldown <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, stats.JumpForce, rb.velocity.z);
            currentJumpCooldown = jumpCooldown;
            currentCoyoteTime = 0f;
        }
    }

    private void UpdateDirectionalMovement()
    {
        // Get client input from the PlayerInput component
        // Normalise it so that diagonal movement isn't faster than cardinal movement
        Vector3 moveInput = sync.InputPacket.walkInput.normalized;
        // Apply move speed modifiers
        moveInput.x *= stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier;
        moveInput.z *= moveInput.z > 0f ? (sync.InputPacket.sprintInput ? (stats.MoveSprintSpeed * stats.MoveSprintMultiplier) : (stats.MoveWalkSpeed * stats.MoveWalkMultiplier)) : (stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier);
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.z;

        // Don't update the rigidbody if we aren't actually going to move
        if (movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
    }

    private void UpdateGroundCheck()
    {
        // How far below the hitbox the ground is measured
        float groundCheckDistance = 0.05f;
        // Check for environment colliders in a small area below our hitbox
        Vector3 p = new Vector3(col.bounds.center.x, col.bounds.center.y - col.bounds.extents.y - (groundCheckDistance / 2f), col.bounds.center.z);
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
}
