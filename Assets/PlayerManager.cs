using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    void Start()
    {
        // If we are running this on the client that owns this player, we create a
        // non-networked instance which allows for prediction
        if (isLocalPlayer)
        {
            Instantiate(localPlayer, transform);
        }
        else if (isLocalPlayer && isServer)
        {
            Instantiate(localPlayer, transform);
        }
        // If we are running this on the server, or a client that doesn't own this specific player,
        // We create a networked player instance
        else if (isServer || !isLocalPlayer)
        {
            Instantiate(networkedPlayer, transform);
        }
    }
}
