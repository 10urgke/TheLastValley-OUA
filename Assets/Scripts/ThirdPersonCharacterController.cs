using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 velocity;
    private float turnSmoothVelocity;

    public GameObject arrow;
    public GameObject arrowPosition;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 10f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity);
        }

        if (arrow != null && arrowPosition != null)
        {
            if(gameObject.tag == "Archer")
            {
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    GameObject newArrow = Instantiate(arrow, arrowPosition.transform.position + transform.forward, arrowPosition.transform.rotation * Quaternion.Euler(0, 90, 0));
                    newArrow.GetComponent<Rigidbody>().AddForce(transform.forward * 2000);
                }
            }
        }

    }

    void Move()
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
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }

        // Apply gravity
        if (characterController.isGrounded)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSmoothTime;
        cameraTransform.Rotate(Vector3.up, mouseX);
    }
}
