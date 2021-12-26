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

    private float cameraRotationLimitUp = 85f;
    private float cameraRotationLimitDown = -90f;

    private CameraMode mode = CameraMode.Unlocked;
    private Vector3 lockedRotation;

    private bool debugCameraModes = true;

    Vector3 CalculateRotation()
    {
        if (head is null)
            return Vector3.zero;

        return new Vector3(head.transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponentInParent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        settings = GetComponent<PlayerSettings>();
        sync = GetComponentInParent<PlayerSynchroniser>();
        head = transform.Find("Head");
    }

    void Update()
    {
        // Rotation is updated every frame locally for responsiveness
        UpdateRotationalMovement();
        DebugCameraModes();
    }

    private void DebugCameraModes()
    {
        if (!debugCameraModes)
            return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            mode = CameraMode.Locked;
            lockedRotation = new Vector3(head.transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            mode = CameraMode.Soft;
            lockedRotation = new Vector3(head.transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            mode = CameraMode.Unlocked;
        }
    }

    private void UpdateRotationalMovement()
    {
        // Get mouse input from PlayerInput component
        // Multiply it by sensitivity
        Vector2 mouseInput = input.InputPacket.mouseInput * settings.Sensitivity;
        // Invert vertical mouse input based on user settings
        mouseInput.x = settings.InvertVerticalMouseInput ? mouseInput.x : -mouseInput.x;

        Vector3 newRot = mode switch
        {
            CameraMode.Soft => UpdateSoftCamera(mouseInput),
            CameraMode.Unlocked => UpdateUnlockedCamera(mouseInput),
            CameraMode.Locked => lockedRotation,
            _ => lockedRotation,
        };
        ApplyRotation(newRot);
        sync.CmdSendRotation(newRot);
    }

    private void ApplyRotation(Vector3 rot)
    {
        // (0, rotY, 0)
        Vector3 horizontalRotation = rot.y * Vector3.up;
        rb.MoveRotation(Quaternion.Euler(horizontalRotation));

        if (head != null)
        {
            float verticalAngle = Mathf.Clamp(rot.x, cameraRotationLimitDown, cameraRotationLimitUp);
            // (rotX, 0, 0)
            Vector3 verticalRotation = verticalAngle * Vector3.right;

            head.transform.localEulerAngles = verticalRotation;
        }
    }

    private float softCap = 1f;
    private float rotationLimit = 30f;
    // Provides softlocked mouse movement
    private Vector3 UpdateSoftCamera(Vector2 mouseInput)
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(mouseInput, softCap);
        Vector3 deltaRotation = new Vector3(clampedInput.y, clampedInput.x, 0f);

        Quaternion newRotation = Quaternion.Euler(deltaRotation) * Quaternion.Euler(new Vector3(head.transform.localEulerAngles.x, transform.localEulerAngles.y, 0f));

        if (Vector3.Distance(newRotation.eulerAngles, lockedRotation) < rotationLimit)
            return newRotation.eulerAngles;
        // TODO: implement
        return lockedRotation;
    }

    private Vector3 UpdateUnlockedCamera(Vector2 mouseInput)
    {
        Vector3 inputVec = mouseInput;
        Vector3 newRotation = CalculateRotation() + inputVec;

        return newRotation;
    }
}

public enum CameraMode
{
    Locked,
    Unlocked,
    Soft
}
