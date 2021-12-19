using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerInput input;
    private PlayerSettings settings;
    private PlayerStats stats;
    private Rigidbody rb;

    private Transform head;

    private float clientCurrentVerticalCameraRotation;

    private bool canMoveCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        settings = GetComponent<PlayerSettings>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
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
            UpdateDirectionalMovement();
        }

        // Rotation is updated each physics update on the server
        if (isServer)
            CmdUpdateRotationalMovement();

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

}
