using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerInput : NetworkBehaviour
{
    private InputPacket input;
    public InputPacket InputPacket { get => input; private set => input = value ; }

    public bool sprinting = false;

    private PlayerSynchroniser sync;

    // Amount of input packets sent from this client to the server.
    // Also represents the ID of the next packet to send
    private int packetsSent = 0;

    void Start()
    {
        sync = GetComponent<PlayerSynchroniser>();
    }

    void Update()
    {
        if (!isLocalPlayer || isServer)
            return;

        // Gather all input data and update it on this client (for movement prediction)
        InputPacket newInput = new InputPacket
        {
            walkInput = UpdateWalkInput(),
            mouseInput = UpdateMouseInput(),
            jumpInput = UpdateJumpInput(),
            id = packetsSent
        };
        input = newInput;

        // Increment packets
        packetsSent++;

        // Update all input on the server (as movement is server-authoritative)
        sync.CmdUpdateInput(newInput);
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
}
