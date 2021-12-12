using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
        float moveSpeed = stats.GetMovementSpeed();
        Vector3 movement = input.GetMovementVector().normalized;

        if (movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
