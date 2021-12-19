using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerInput input;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;

    private bool canMoveCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        settings = GetComponent<PlayerSettings>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Movement is server-authoritative
        if (!(isServer || isLocalPlayer))
            return;

        // Get movespeed from the PlayerStats component
        // Get client input from the PlayerInput component
        // Normalise it so that diagonal movement isn't faster than cardinal movement
        Vector3 moveInput = input.GetMovementVector().normalized;
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

}
