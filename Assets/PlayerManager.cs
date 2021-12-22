using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

// In order to utilise client-side prediction, 2 versions of the player must exist
// For the client that owns a player, it must have a non-networked version that it
// can control and use for prediction.
// The local client owns only a copy of the non-networked player, and sends input 
// to the server while simultaneously predicting movement.
// External clients own only a copy of the network player, which is constantly
// updated and simulated on the server based on it's respective player's inputs.
public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject localPlayer, networkedPlayer;

    // The rigidbody of the player contained within this manager
    private Rigidbody rb;

    private PlayerMovementPrediction mv;
    private PlayerMovementServer mvs;

    [SerializeField]
    private float resendDelay = 1f;
    private float currentDelay = 1f;

    // Instantiate the correct version of player
    void Start()
    {
        // If we are running this on the client that owns this player, we create a
        // non-networked instance which allows for prediction
        if (isLocalPlayer && !isServer)
        {
            Instantiate(localPlayer, transform);
            mv = GetComponentInChildren<PlayerMovementPrediction>();
        }
        // If we are running this on the server, or a client that doesn't own this specific player,
        // We create a networked player instance
        else if (isServer || !isLocalPlayer)
        {
            Instantiate(networkedPlayer, transform);
        }

        if (isServer)
        {
            rb = GetComponentInChildren<Rigidbody>();
            mvs = GetComponentInChildren<PlayerMovementServer>();

        }
    }

    void Update()
    {
        if (!isServer)
            return;

        if (currentDelay > 0)
            currentDelay -= Time.deltaTime;
        if(currentDelay <= 0)
        {
            currentDelay = resendDelay;
            TargetSendStateUpdate(new PlayerStatePacket(rb.position));
        }
    }

    // This function is only run on the local client
    [TargetRpc]
    void TargetSendStateUpdate(PlayerStatePacket s)
    {
        mv?.UpdateServerState(s);
    }

    // This is used to update rotation on the server as it is client-authoritative
    [Command]
    public void CmdSendRotation(Vector3 r)
    {
        mvs?.RecieveRotationalMovement(r);
    }
}

public struct PlayerStatePacket : NetworkMessage
{
    public Vector3 position;

    public PlayerStatePacket(Vector3 p)
    {
        position = p;
    }

    public bool Equals(PlayerStatePacket s)
    {
        return position == s.position;
    }
}
