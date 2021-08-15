using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 0.1f;
    [SerializeField] private float sprintSpeed = 0.3f;
    [SerializeField] private float xSensivity = 1f;
    [SerializeField] private float ySensivity = 1f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private Animator animator;
    [SerializeField] private float timeBetweenShoots = 1.0f;

    private float minimumX = -90;
    private float maximumX = 90;
    private float shootTime;

    private float x, z;

    private float moveSpeed;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isShooting = false;

    private Quaternion cameraRot;
    private Quaternion characterRot;

    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private Camera mainCamera;

    private bool lockCursor=true;
    private bool cursorIsLocked=true;
    private static readonly int Aiming = Animator.StringToHash("aiming");
    private static readonly int ReloadOutOfAmmo = Animator.StringToHash("reloadOutOfAmmo");
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int AimShoot = Animator.StringToHash("aimShoot");
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int KnifeAttack1 = Animator.StringToHash("knifeAttack1");
    private static readonly int ReloadAmmoLeft = Animator.StringToHash("reloadAmmoLeft");

    private void Start()
    {
        rigidbody = GetComponent <Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mainCamera=Camera.main;
        cameraRot = mainCamera.transform.localRotation;
        characterRot = transform.localRotation;
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        AnimationHandler();

        if (IsGrounded() && isJumping)
        {
            isJumping = false;
            AudioManager.PlayLandAudio();
        }
    }

    private void AnimationHandler()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            animator.SetBool(Aiming, !animator.GetBool(Aiming));
        }
        if (Input.GetButtonUp("Fire2"))
        {
            animator.SetBool(Aiming, !animator.GetBool(Aiming));
        }

        if (Input.GetButton("Fire1"))
        {
            ShootSequence();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isShooting = false;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger(KnifeAttack1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger(ReloadOutOfAmmo);
        }

        if (Math.Abs(x) > 0 || Math.Abs(z) > 0)
        {
            if (isRunning)
            {
                if (!animator.GetBool(Run))
                {
                    animator.SetBool(Run, true);
                    AudioManager.PlayRunAudio();
                }

            } else
            {
                if (isJumping)
                {
                    if(animator.GetBool(Walk))
                        animator.SetBool(Walk, false);
                    if(animator.GetBool(Run))
                        animator.SetBool(Run,false);
                } else if(!isJumping)
                {
                    if (!animator.GetBool(Walk))
                    {
                        animator.SetBool(Walk, true);
                        AudioManager.PlayWalkAudio();
                    }

                    if (animator.GetBool(Run))
                    {
                        animator.SetBool(Run, false);
                        AudioManager.PlayWalkAudio();
                    }
                }



            }
        } else if (animator.GetBool(Walk) || animator.GetBool(Run))
        {
            AudioManager.StopPlayerAudio();
            animator.SetBool(Walk, false);
            animator.SetBool(Run, false);
        }
    }

    private void ShootSequence()
    {
        if(Time.time<shootTime)    return;

        animator.SetTrigger(animator.GetBool(Aiming) ? AimShoot : Shoot);
        AudioManager.PlayShootAudio();
        //AudioManager.PlayCasingAudio();
        isShooting = true;
        shootTime = Time.time + timeBetweenShoots;
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
        if (Input.GetKey(KeyCode.LeftShift) && !isJumping && !isShooting ) //if runninig animation need to disable while jump add !iJumping to this line
        {
            moveSpeed = sprintSpeed;
            isRunning = true;
        } else
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }
         x = Input.GetAxis("Horizontal") * moveSpeed;
         z = Input.GetAxis("Vertical") * moveSpeed;

        transform.position += mainCamera.transform.forward * z + mainCamera.transform.right * x;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigidbody.AddForce(0, jumpForce, 0);
            AudioManager.PlayJumpAudio();
            isJumping = true;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hitInfo;
        return Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out hitInfo,
            (capsuleCollider.height * 0.5f) - capsuleCollider.radius + 0.01f);
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
