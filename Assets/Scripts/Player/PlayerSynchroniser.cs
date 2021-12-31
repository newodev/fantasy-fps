using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//
public class PlayerSynchroniser : NetworkBehaviour
{
    #region component references
    private Rigidbody rb;

    private PlayerMovementPrediction localPlayerMovement;
    private PlayerMovementServer serverPlayerMovement;
    #endregion

    private InputPacket input;
    public InputPacket InputPacket { get => input; private set => input = value; }

    // Gather correct components depending on whether we are a client, or a server
    // (AKA whether we hold a local player)
    void Start()
    {
        if (isLocalPlayer && !isServer)
        {
            localPlayerMovement = GetComponentInChildren<PlayerMovementPrediction>();
        }

        if (isServer)
        {
            rb = GetComponentInChildren<Rigidbody>();
            serverPlayerMovement = GetComponentInChildren<PlayerMovementServer>();
        }
    }

    // A command sent by the client to the server, telling to update input
    [Command]
    public void CmdUpdateInput(InputPacket i)
    {
        // Clamp walk input so that speedhacks won't work
        i.walkInput.x = Mathf.Clamp(i.walkInput.x, -1f, 1f);
        i.walkInput.y = 0f;
        i.walkInput.z = Mathf.Clamp(i.walkInput.z, -1f, 1f);

        // Update the input from the client on the server
        // We do this because we only previously updated it on the client
        input = i;

        AcknowledgeInputPacket(i.id);
    }

    // Used by the server to acknowledge an input packet, and send the server state after applying that input
    // TODO: we actually send state based on input packet from LAST frame. this will need to be adjusted, otherwise, slight lag will result in rubber banding
    public void AcknowledgeInputPacket(int inputPacketID)
    {
        TargetSendStateUpdate(new PlayerStatePacket
        {
            position = rb.position
        }, inputPacketID);
    }

    // This function is only run on the local client
    [TargetRpc]
    void TargetSendStateUpdate(PlayerStatePacket s, int inputPacketID)
    {
        localPlayerMovement?.RecieveServerAcknowledge(s, inputPacketID);
    }

    // This is used to update rotation on the server as it is client-authoritative
    [Command]
    public void CmdSendRotation(Vector3 r)
    {
        serverPlayerMovement?.RecieveRotationalMovement(r);
    }
}

public struct PlayerStatePacket
{
    public Vector3 position;

    public bool Equals(PlayerStatePacket s)
    {
        return position == s.position;
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
