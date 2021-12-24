using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    #region component references
    private PlayerInput input;
    private Rigidbody rb;
    private PlayerSynchroniser sync;
    private PlayerSettings settings;

    private Transform head;
    #endregion

    private float currentVerticalCameraRotation;
    private float cameraRotationLimitUp = 85f;
    private float cameraRotationLimitDown = -90f;

    private CameraMode mode = CameraMode.Unlocked;
    private Vector3 lockedRotation;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponentInParent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        settings = GetComponent<PlayerSettings>();
        sync = GetComponentInParent<PlayerSynchroniser>();
    }

    void Update()
    {
        // Rotation is updated every frame locally for responsiveness
        UpdateRotationalMovement();
    }

    private void UpdateRotationalMovement()
    {
        // Get mouse input from PlayerInput component
        // Multiply it by sensitivity
        Vector2 mouseInput = input.InputPacket.mouseInput * settings.Sensitivity;

        Vector3 newRot = mode switch
        {
            CameraMode.Soft => UpdateSoftCamera(mouseInput),
            CameraMode.Unlocked => UpdateUnlockedCamera(mouseInput),
            CameraMode.Locked => lockedRotation,
            _ => lockedRotation,
        };

        sync.CmdSendRotation(newRot);
    }

    // Provides softlocked mouse movement
    private Vector3 UpdateSoftCamera(Vector2 mouseInput)
    {
        // TODO: implement
        return lockedRotation;
    }

    private Vector3 UpdateUnlockedCamera(Vector2 mouseInput)
    {
        Vector3 deltaHorizontal = new Vector3(0f, mouseInput.x, 0f);
        Quaternion horizontalRotation = rb.rotation * Quaternion.Euler(deltaHorizontal);

        rb.MoveRotation(horizontalRotation);
        if (head != null)
        {
            currentVerticalCameraRotation -= mouseInput.y;
            currentVerticalCameraRotation = Mathf.Clamp(currentVerticalCameraRotation, cameraRotationLimitDown, cameraRotationLimitUp);
            Vector3 verticalRotation = new Vector3(currentVerticalCameraRotation, 0f, 0f);

            head.transform.localEulerAngles = verticalRotation;
        }
        // TODO: These are the wrong way around? refactor to right order
        return new Vector3(horizontalRotation.eulerAngles.y, currentVerticalCameraRotation, 0f);
    }
}

public enum CameraMode
{
    Locked,
    Unlocked,
    Soft
}
