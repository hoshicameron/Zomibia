using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

public class FPController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 0.1f;
    [SerializeField] private float sprintSpeed = 0.3f;
    [SerializeField] private float jumpForce = 300f;
    [Header("Mouse Sensitivity")]
    [SerializeField] private float xSensitivity = 1f;
    [SerializeField] private float ySensitivity = 1f;
    [Header("Aim Details")]
    [SerializeField] private float zoomedFOV;

    [SerializeField] private Animator animator;
    [SerializeField] private float timeBetweenShoots = 1.0f;
    [SerializeField] private float reloadTimerOutOfBullet;
    [SerializeField] private float reloadTimerBulletLeft;

    public ForceMode forceMode;

    private float minimumX = -90;
    private float maximumX = 90;
    private float shootTime;

    private float hMove, vMove;
    private float originalFOV;

    private float moveSpeed;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isShooting = false;
    private bool isReloading=false;
    private bool isAiming=false;
    private Quaternion cameraRot;
    private Quaternion characterRot;

    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private Camera mainCamera;
    private Health health;
    private Ammo ammo;

    private bool lockCursor=true;
    private bool cursorIsLocked=true;
    private static readonly int Aiming = Animator.StringToHash("aiming");
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int AimShoot = Animator.StringToHash("aimShoot");
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int KnifeAttack1 = Animator.StringToHash("knifeAttack1");
    private static readonly int ReloadAmmoLeft = Animator.StringToHash("reloadAmmoLeft");
    private static readonly int ReloadOutOfAmmo = Animator.StringToHash("reloadOutOfAmmo");

    private IEnumerator reloadCoroutine;

    private void Start()
    {
        rigidbody = GetComponent <Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        health = GetComponent<Health>();
        ammo = GetComponent<Ammo>();
        mainCamera=Camera.main;
        cameraRot = mainCamera.transform.localRotation;
        characterRot = transform.localRotation;
        moveSpeed = walkSpeed;
        originalFOV = mainCamera.fieldOfView;

        ammo.OnMagazineReloaded_NotFull+=AmmoOnMagazineReloadedNotFull;
        ammo.OnMagazineReload_Full+=Ammo_OnMagazineReloadFull;
    }

    private void Ammo_OnMagazineReloadFull(object sender, EventArgs e)
    {
        if(isAiming)
            AimOut();

        animator.SetTrigger(ReloadAmmoLeft);
        AudioManager.PlayerReloadAmmoLeftAudio();
        reloadCoroutine = ReloadWait(reloadTimerBulletLeft);
        StartCoroutine(reloadCoroutine);
    }

    private void AmmoOnMagazineReloadedNotFull(object sender, EventArgs e)
    {
        if(isAiming)
            AimOut();

        if (!ammo.IsMagazineEmpty())
        {
            animator.SetTrigger(ReloadOutOfAmmo);
            AudioManager.PlayerReloadOutOfAmmoAudio();

            reloadCoroutine = ReloadWait(reloadTimerOutOfBullet);
            StartCoroutine(reloadCoroutine);
        }
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
        if (Input.GetButtonDown("Fire2") && !isReloading )
        {
            AimIn();
        }

        if (Input.GetButton("Fire2"))
        {
            isRunning = false;

        }
        if (Input.GetButtonUp("Fire2"))
        {
            AimOut();
        }

        if (Input.GetButton("Fire1") && !isReloading)
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
            ammo.ReFillMagazine();
        }

        if (Math.Abs(hMove) > 0 || Math.Abs(vMove) > 0)
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
    private void AimOut()
    {
        isAiming = false;
        animator.SetBool(Aiming, false);
        mainCamera.fieldOfView = originalFOV;
    }

    private void AimIn()
    {
        isAiming = true;
        animator.SetBool(Aiming, true);
        mainCamera.fieldOfView = zoomedFOV;
    }

    IEnumerator ReloadWait(float timer)
    {
        isReloading = true;
        yield return new WaitForSeconds(timer);
        isReloading = false;
    }

    private void ShootSequence()
    {
        if(Time.time<shootTime)    return;

        animator.SetTrigger(animator.GetBool(Aiming) ? AimShoot : Shoot);
        if (!ammo.IsOutOfAmmo())
        {
            if (ammo.IsMagazineEmpty())
            {
                ammo.ReFillMagazine();
            } else
            {
                AudioManager.PlayShootAudio();
                ammo.SpendAmmo();
                //AudioManager.PlayCasingAudio();
            }

        } else
        {
            AudioManager.PlayTriggerAudio();
        }
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
        float yRot = Input.GetAxis("Mouse X") * ySensitivity;
        float xRot = Input.GetAxis("Mouse Y") * xSensitivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        mainCamera.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;
    }

    private void MovementHandler()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isJumping && !isShooting && !isAiming) //if runninig animation need to disable while jump add !iJumping to this line
        {
            moveSpeed = sprintSpeed;
            isRunning = true;
        } else
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }

        //Getting the direction to move through player input
        hMove = Input.GetAxis("Horizontal");
        vMove = Input.GetAxis("Vertical");

        //Get directions relative to camera
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        // Project forward and right direction on the horizontal plane (not up and down), then
        // normalize to get magnitude of 1
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction=  forward* vMove +  right* hMove;

        // Set the direction's magnitude to 1 so that it does not interfere with the movement speed
        direction.Normalize();
        print(direction * moveSpeed * Time.fixedDeltaTime);

        rigidbody.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ammo") && ammo.CanAddAmmo())
        {
            Debug.Log("Ammo Collected!");
            AudioManager.PlayAmmoPickupAudio();
            ammo.AddAmmo(100);
            if (ammo.IsMagazineEmpty())
            {
                ammo.ReFillMagazine();
            }

            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Aid") && health.CanAddHealth())
        {
            Debug.Log("Aid Collected!");
            AudioManager.PlayAidPickupAudio();
            health.AddHealth(20);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Lava")) return;

        health.GetDamage(10);

    }

    /*private void OnCollisionStay(Collision other)
    {
        if (!other.gameObject.CompareTag("Lava")) return;
        health.GetDamage(1);
    }*/
}
