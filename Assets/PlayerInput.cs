using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerInput : NetworkBehaviour
{
    private InputPacket input;

    // Public getter functions
    public Vector3 GetWalkInputVector() => input.walkInput;
    public Vector3 GetMouseInputVector() => input.mouseInput;
    public bool GetJumpKeyPressed() => input.jumpInput;

    public bool sprinting = false;

    // Amount of input packets sent from this client to the server.
    // Also represents the ID of the next packet to send
    private int packetsSent = 0;
    // Incremented when a packet is sent, decremented when it is acknowledged by the server.
    private int packetsUnacknowledged = 0;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        // Gather all input data and update it on this client (for movement prediction)
        InputPacket newInput = new InputPacket
        {
            walkInput = UpdateWalkInput(),
            mouseInput = UpdateMouseInput(),
            jumpInput = UpdateJumpInput()
        };
        input = newInput;

        // Update all input on the server (as movement is server-authoritative)
        CmdUpdateInput(newInput);
    }

    private Vector2 UpdateMouseInput()
    {
        // Get mouse input of each axis
        float horizontalMouseInput = Input.GetAxisRaw("Mouse X");
        float verticalMouseInput = Input.GetAxisRaw("Mouse Y");

        // Combine input into a vector2
        Vector2 mouseInput = new Vector2(horizontalMouseInput, verticalMouseInput);

        return mouseInput;
    }

    private Vector3 UpdateWalkInput()
    {
        // Get wasd input of each axis (WS, AD)
        float verticalWalkInput = Input.GetAxisRaw("Vertical");
        float horizontalWalkInput = Input.GetAxisRaw("Horizontal");

        // Combine input axes into vector. Y value is 0 because the player doesn't move vertically
        Vector3 walkInput = new Vector3(horizontalWalkInput, 0f, verticalWalkInput);

        return walkInput;
    }

    private bool UpdateJumpInput()
    {
        bool jumpInput = Input.GetKey(KeyCode.Space);
        return jumpInput;
    }

    // A command sent by the client to the server, telling to update input
    [Command]
    private void CmdUpdateInput(InputPacket i)
    {
        // Clamp walk input so that speedhacks won't work
        i.walkInput.x = Mathf.Clamp(i.walkInput.x, -1f, 1f);
        i.walkInput.y = 0f;
        i.walkInput.z = Mathf.Clamp(i.walkInput.z, -1f, 1f);

        // Update the input from the client on the server
        // We do this because we only previously updated it on the client
        input = i;
    }
}

public struct InputPacket
{
    // A vector from (-1, 0, -1) to (1, 0, 1) based on axis (default WASD) input
    // This vector is sent from the client and saved on the server
    public Vector3 walkInput;
    // A vector describing this frame's mouse input
    // Mouse movement is client authoritative
    public Vector2 mouseInput;
    // Describes whether jump key (default SPACE) was pressed in this frame
    public bool jumpInput;
    // Describes whether sprint key (default SHIFT) was pressed in this frame
    public bool sprintInput;

    // ID of this packet. Used by client to reconciliate.
    public int id;
}
