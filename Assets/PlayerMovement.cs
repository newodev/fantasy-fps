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
        if (!isServer)
            return;
        float moveSpeed = stats.GetMovementSpeed();
        Vector3 movement = input.GetMovementVector().normalized;

        if (movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

}
