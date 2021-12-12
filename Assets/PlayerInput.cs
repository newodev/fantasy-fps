using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // A vector from (-1, 0, -1) to (1, 0, 1) based on axis (WASD) input
    private Vector3 inputVector;

    void Update()
    {
        // Get input of each axis (WS, AD)
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        bool baat;
        baat = Input.GetKey(KeyCode.Space);

        // Combine input axes into vector. Y value is 0 because the player doesn't move vertically
        inputVector = new Vector3(horizontalInput, 0f, verticalInput);
    }

    public Vector3 GetMovementVector()
    {
        return inputVector;
    }
}
