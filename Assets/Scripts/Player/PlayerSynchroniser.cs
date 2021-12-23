using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//
public class PlayerSynchroniser : NetworkBehaviour
{
    // The rigidbody of the player contained within this manager
    private Rigidbody rb;

    private PlayerMovementPrediction mv;
    private PlayerMovementServer mvs;

    private int packetsRecieved = 0;
    private int packetsAcknowledged = 0;

    private InputPacket input;
    public InputPacket InputPacket { get => input; private set => input = value; }

    // Gather correct components depending on whether we are a client, or a server
    // (AKA whether we hold a local player)
    void Start()
    {
        if (isLocalPlayer && !isServer)
        {
            mv = GetComponentInChildren<PlayerMovementPrediction>();
        }

        if (isServer)
        {
            rb = GetComponentInChildren<Rigidbody>();
            mvs = GetComponentInChildren<PlayerMovementServer>();
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
        packetsRecieved++;

        AcknowledgeInputPacket(i.id);
    }

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
        mv?.RecieveServerAcknowledge(s, inputPacketID);
    }

    // This is used to update rotation on the server as it is client-authoritative
    [Command]
    public void CmdSendRotation(Vector3 r)
    {
        mvs?.RecieveRotationalMovement(r);
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
