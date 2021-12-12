using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInput : NetworkBehaviour
{
    // A vector from (-1, 0, -1) to (1, 0, 1) based on axis (WASD) input
    // This vector is sent from the client and saved on the server
    private Vector3 clientInputVector;

    void Update()
    {
        if (!isLocalPlayer)
            return;
        // Get input of each axis (WS, AD)
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Combine input axes into vector. Y value is 0 because the player doesn't move vertically
        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput);

        CmdUpdateInput(inputVector);
    }

    // A command sent by the client to the server, telling to update input
    [Command]
    private void CmdUpdateInput(Vector3 input)
    {
        // Clamp input so that speedhacks won't work
        input.x = Mathf.Clamp(input.x, -1f, 1f);
        input.y = 0f;
        input.z = Mathf.Clamp(input.z, -1f, 1f);

        // Update the input from the client on the server
        clientInputVector = input;
    }

    public Vector3 GetMovementVector()
    {
        return clientInputVector;
    }
}
