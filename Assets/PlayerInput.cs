using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerInput : NetworkBehaviour
{
    // A vector from (-1, 0, -1) to (1, 0, 1) based on axis (WASD) input
    // This vector is sent from the client and saved on the server
    private Vector3 clientWalkInputVector;
    // A vector describing this frame's mouse input
    // Mouse movement is client authoritative
    private Vector2 clientMouseInputVector;

    // Public getter functions
    public Vector3 GetWalkInputVector() => clientWalkInputVector;
    public Vector3 GetMouseInputVector() => clientMouseInputVector;

    public bool sprinting = false;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        Vector3 walkInput = UpdateWalkInput();
        Vector2 mouseInput = UpdateMouseInput();

        // Update all input on the server (as movement is server-authoritative)
        CmdUpdateInput(walkInput, mouseInput);
    }

    private Vector2 UpdateMouseInput()
    {
        // Get mouse input of each axis
        float horizontalMouseInput = Input.GetAxisRaw("Mouse X");
        float verticalMouseInput = Input.GetAxisRaw("Mouse Y");

        // Combine input into a vector2
        Vector2 mouseInput = new Vector2(horizontalMouseInput, verticalMouseInput);

        // Update movement vector on this client (as it is client-authoritative)
        clientMouseInputVector = mouseInput;

        return mouseInput;
    }

    private Vector3 UpdateWalkInput()
    {
        // Get wasd input of each axis (WS, AD)
        float verticalWalkInput = Input.GetAxisRaw("Vertical");
        float horizontalWalkInput = Input.GetAxisRaw("Horizontal");

        // Combine input axes into vector. Y value is 0 because the player doesn't move vertically
        Vector3 walkInput = new Vector3(horizontalWalkInput, 0f, verticalWalkInput);

        // Update movement vector on this client (for prediction)
        clientWalkInputVector = walkInput;

        return walkInput;
    }

    // A command sent by the client to the server, telling to update input
    [Command]
    private void CmdUpdateInput(Vector3 walkInput, Vector2 mouseInput)
    {
        // Clamp input so that speedhacks won't work
        walkInput.x = Mathf.Clamp(walkInput.x, -1f, 1f);
        walkInput.y = 0f;
        walkInput.z = Mathf.Clamp(walkInput.z, -1f, 1f);

        // Update the input from the client on the server
        // We do this because we only previously updated it on the client
        clientWalkInputVector = walkInput;
        clientMouseInputVector = mouseInput;
    }
}
