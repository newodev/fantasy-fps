using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerInput input;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;
    private new Collider collider;

    private Transform head;

    private float clientCurrentVerticalCameraRotation;

    private bool canMoveCamera = true;

    private float jumpCooldown;

    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        settings = GetComponent<PlayerSettings>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        collider = transform.Find("Model").GetComponent<Collider>();
        head = transform.Find("Head");
    }

    void Update()
    {
        // Rotation is updated every frame locally for responsiveness
        if (isLocalPlayer)
            UpdateRotationalMovementLocal();
    }

    void FixedUpdate()
    {
        // Movement is server-authoritative
        if (isServer || isLocalPlayer)
        {
            UpdateGroundCheck();
            UpdateDirectionalMovement();
        }


        if (isLocalPlayer)
        {
            UpdateRotationalMovementLocal();
            UpdateJump();
            CmdUpdateRotationalMovement();
            CmdUpdateJump();
        }

    }

    private void UpdateRotationalMovementLocal()
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

    [Command]
    private void CmdUpdateRotationalMovement()
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

    private void UpdateDirectionalMovement()
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

    private void UpdateGroundCheck()
    {
        float groundCheckDistance = 0.05f;
        Vector3 p = new Vector3(collider.bounds.center.x, collider.bounds.center.y - collider.bounds.extents.y - (groundCheckDistance / 2f), collider.bounds.center.z);

        onGround = Physics.OverlapSphere(p, groundCheckDistance / 2f).Where(x => x.tag == "Environment").Count() > 0;
    }

    private void UpdateJump()
    {
        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.fixedDeltaTime;
        }
        if (onGround && input.GetJumpKeyPressed() && jumpCooldown <= 0)
        {
            rb.AddForce(new Vector3(0f, stats.JumpForce, 0f), ForceMode.Impulse);
            jumpCooldown = 1f;
        }
    }

    [Command]
    private void CmdUpdateJump()
    {
        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.fixedDeltaTime;
        }
        if (onGround && input.GetJumpKeyPressed() && jumpCooldown <= 0)
        {
            rb.AddForce(new Vector3(0f, stats.JumpForce, 0f), ForceMode.Impulse);
            jumpCooldown = 1f;
        }
    }
}
