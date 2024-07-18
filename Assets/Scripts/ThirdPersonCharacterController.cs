using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ThirdPersonCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 1f;

    [Header("Animation Settings")]
    protected PlayerAnimationManager animationManager;
    [SerializeField] private float animationWalkSpeed = 0f;

    [Space]
    private CharacterController characterController;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    public bool sprintBlock;
    public bool isCarrying;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animationManager = GetComponent<PlayerAnimationManager>();
    }

    protected virtual void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            float currentSpeed = speed;
            if (sprintBlock)
                currentSpeed *= .5f;

            if (Input.GetAxis("Sprint") > 0 && !sprintBlock)
            {
                currentSpeed = sprintSpeed;
                //smoothing movement animation
                if (animationWalkSpeed < 1f)
                    animationWalkSpeed += Time.deltaTime;
                animationManager.SetSpeed(animationWalkSpeed);

            }
            else
            {
                if (animationWalkSpeed > 0.52f)
                    animationWalkSpeed -= Time.deltaTime;
                else if (animationWalkSpeed < 0.48f)
                    animationWalkSpeed += Time.deltaTime;
                animationManager.SetSpeed(animationWalkSpeed);
            }

            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            if (animationWalkSpeed > 0f)
            {
                animationWalkSpeed -= Time.deltaTime;
                animationManager.SetSpeed(animationWalkSpeed);
            }

        }

        if (characterController.isGrounded)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (animationManager.animator.GetBool("Carry") || animationManager.animator.GetBool("Second"))
            return;
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            //make jump
            //velocity.y += Mathf.Sqrt(-2f * gravity);
            animationManager.SetTrigger("Jump");
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraTransform.Rotate(Vector3.up, mouseX);
    }
}