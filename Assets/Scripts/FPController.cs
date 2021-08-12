using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float xSensivity = 1f;
    [SerializeField] private float ySensivity = 1f;
    [SerializeField] private float jumpForce = 300f;

    private float minimumX = -90;
    private float maximumX = 90;

    private Quaternion cameraRot;
    private Quaternion characterRot;

    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private Camera mainCamera;

    private bool lockCursor=true;
    private bool cursorIsLocked=true;

    private void Start()
    {
        rigidbody = GetComponent <Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mainCamera=Camera.main;
        cameraRot = mainCamera.transform.localRotation;
        characterRot = transform.localRotation;
    }

    private void FixedUpdate()
    {
        RotationHandler();
        MovementHandler();

        UpdateCursorLock();
    }

    private void RotationHandler()
    {
        float yRot = Input.GetAxis("Mouse X") * ySensivity;
        float xRot = Input.GetAxis("Mouse Y") * xSensivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        mainCamera.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;
    }

    private void MovementHandler()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        transform.position += mainCamera.transform.forward * z + mainCamera.transform.right * x;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigidbody.AddForce(0, jumpForce, 0);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out hitInfo,
            (capsuleCollider.height * 0.5f) - capsuleCollider.radius + 0.1f))
        {
            return true;
        }

        return false;
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion quaternion)
    {
        //Normalize quaternion
        quaternion.x /= quaternion.w;
        quaternion.y /= quaternion.w;
        quaternion.z /= quaternion.w;
        quaternion.w = 1.0f;

        //Convert quaternion to eulerAngle
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x);

        //Clamp the angle
        angleX = Mathf.Clamp(angleX, minimumX, maximumX);

       //Turn back eulerAngle to quaternion
        quaternion.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return quaternion;
    }


    private void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            cursorIsLocked = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            cursorIsLocked = true;
        }
        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
