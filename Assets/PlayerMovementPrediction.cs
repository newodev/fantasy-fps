using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

// Represents the position of the player after applying the input packet
public struct StateSnapshot
{
    public InputPacket packet;
    public Vector3 position;
}
// This component executes solely on the player owned by the local client, and performs client-side prediction.
public class PlayerMovementPrediction : MonoBehaviour
{
    private PlayerInput input;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;
    private PlayerSynchroniser sync;
    private new Collider collider;

    private Transform head;

    private float clientCurrentVerticalCameraRotation;
    private float cameraRotationLimitUp = 85f;
    private float cameraRotationLimitDown = -90f;

    private bool canMoveCamera = true;

    private float jumpCooldown = 1.5f;
    private float currentJumpCooldown;

    private bool onGround;

    private float coyoteTime = 0.2f;
    private float currentCoyoteTime;

    // How far the player's server position can be from local position before reconciliating
    [SerializeField]
    private float reconciliationThreshold = 0.5f;
    private float reconciliationTime = 1f;
    private float reconciliationDuration = 1f;

    // The most recent server state recieved
    private List<StateSnapshot> positions = new List<StateSnapshot>();
    public void RecieveServerAcknowledge(PlayerStatePacket s, int inputPacketID)
    {
        Vector3 nullY = new Vector3(1, 0, 1);
        // This can occur in the player's first frame of existence
        // Return out for safety
        if (positions.Count == 0)
            return;

        // Verify that our local state after predicting with the input with this ID
        // was at least very close to what the server has
        // TODO: later maybe clean this up
        IEnumerable<StateSnapshot> localState = positions.Where(x => x.packet.id == inputPacketID);
        if (localState.Count() == 0)
            return;
        int localStateIndex = positions.IndexOf(localState.First());

        int inputsToSimulate = positions.Count - (localStateIndex + 1);
        // TODO: This part probably shouldnt have to exist.
        if (inputsToSimulate < 1)
        {
            return;
        }

        if (Vector3.Distance(positions[localStateIndex].position, s.position) > reconciliationThreshold)
        {
            Debug.Log($"s: {s.position} l: {positions[localStateIndex].position}");

            Debug.Log("Reconciliating");

            // Here we must re-simulate any further inputs since the one that was just acknowledged.
            transform.position = s.position;

            Vector3 startPos = transform.position;
            Vector3 newPos = transform.position;
            for (int i = localStateIndex + 1; i < inputsToSimulate; i++)
            {
                Vector3 currentPos = newPos;
                PredictGroundCheck();
                newPos = PredictDirectionalMovement(positions[i].packet, currentPos);
                UpdateJump();
            }
            Debug.Log($"Simulated x{inputsToSimulate}. a: {startPos} b: {newPos}");


            transform.position = newPos;
        }
        else
            Debug.Log("Didn't Reconciliate");
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponentInParent<PlayerInput>();
        sync = GetComponentInParent<PlayerSynchroniser>();
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
    }

    void FixedUpdate()
    {
        Simulate(input.InputPacket);

        // Save snapshot to solve desync with
        positions.Add(new StateSnapshot { position = transform.position, packet = input.InputPacket });
        if (positions.Count > 20)
        {
            positions.RemoveAt(0);
        }
    }

    void Simulate(InputPacket i)
    {
        Vector3 currentPos = transform.position;
        PredictGroundCheck();
        Vector3 newPos = PredictDirectionalMovement(i, currentPos);
        UpdateJump();

        // Don't update the rigidbody if we aren't actually going to move
        if (newPos != currentPos)
        {
            rb.MovePosition(newPos);
        }
    }


    private void PredictRotationalMovement()
    {
        if (!canMoveCamera)
            return;

        // Get mouse input from PlayerInput component
        // Multiply it by sensitivity
        Vector2 mouseInput = input.InputPacket.mouseInput * settings.Sensitivity;

        Vector3 deltaHorizontal = new Vector3(0f, mouseInput.x, 0f);
        Quaternion horizontalRotation = rb.rotation * Quaternion.Euler(deltaHorizontal);

        rb.MoveRotation(horizontalRotation);
        if (head != null)
        {
            clientCurrentVerticalCameraRotation -= mouseInput.y;
            clientCurrentVerticalCameraRotation = Mathf.Clamp(clientCurrentVerticalCameraRotation, cameraRotationLimitDown, cameraRotationLimitUp);
            Vector3 verticalRotation = new Vector3(clientCurrentVerticalCameraRotation, 0f, 0f);

            head.transform.localEulerAngles = verticalRotation;
        }

        // TODO: These are the wrong way around? refactor to right order
        sync.CmdSendRotation(new Vector3(horizontalRotation.eulerAngles.y, clientCurrentVerticalCameraRotation, 0f));
    }

    private Vector3 PredictDirectionalMovement(InputPacket i, Vector3 currentPos)
    {
        // Get client input from the PlayerInput component
        // Normalise it so that diagonal movement isn't faster than cardinal movement
        Vector3 moveInput = i.walkInput.normalized;
        // Apply move speed modifiers
        moveInput.x *= stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier;
        moveInput.z *= moveInput.z > 0f ? (i.sprintInput ? (stats.MoveSprintSpeed * stats.MoveSprintMultiplier) : (stats.MoveWalkSpeed * stats.MoveWalkMultiplier)) : (stats.MoveStrafeSpeed * stats.MoveStrafeMultiplier);
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.z;

        return rb.position + movement * Time.fixedDeltaTime;
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


    // TODO: Refactor jump to be entirely custom forces
    private void UpdateJump()
    {
        // Decrement jump cooldown
        if (currentJumpCooldown > 0)
        {
            currentJumpCooldown -= Time.fixedDeltaTime;
        }
        // Standard jump check
        if (onGround && input.InputPacket.jumpInput && currentJumpCooldown <= 0)
        {
            rb.AddForce(new Vector3(0f, stats.JumpForce, 0f), ForceMode.Impulse);
            currentJumpCooldown = jumpCooldown;
        }
        // Coyote jump check
        else if (currentCoyoteTime > 0 && input.InputPacket.jumpInput && currentJumpCooldown <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, stats.JumpForce, rb.velocity.z);
            currentJumpCooldown = jumpCooldown;
            currentCoyoteTime = 0f;
        }
    }
}
