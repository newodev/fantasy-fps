using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerStats stats;
    private PlayerInput input;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Movement is server-authoritative
        if (!(isServer || isLocalPlayer))
            return;

        // Get movespeed from the PlayerStats component
        float moveSpeed = stats.GetMovementSpeed();
        // Get client input from the PlayerInput component
        // Normalise it so that diagonal movement isn't faster than cardinal movement
        Vector3 movement = input.GetMovementVector().normalized;

        // Don't update the rigidbody if we aren't actually going to move
        if (movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

}
