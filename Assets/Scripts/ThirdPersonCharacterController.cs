using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.Demo.PunBasics;
public class ThirdPersonCharacterController : MonoBehaviourPun
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public float jumpDuration = .5f;
    public float jumpDelay = .5f;
    public float jumpHeight = 2.0f;

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
    public bool isDead = false;

    [Space]
    public float health = 100f;
    public Slider heathBarSelf;
    public Slider heathBarForOthers;
    public GameObject helpPanel;
    public TextMeshProUGUI timerText;
    public GameManager gameManager;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animationManager = GetComponent<PlayerAnimationManager>();
         
        heathBarSelf.maxValue = health;
        heathBarForOthers.maxValue = health;
        AdjustHealthBars();
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.SetTimerText(timerText);
    }

    protected virtual void Update()
    {         
        if (isDead) 
            return;
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
            animationManager.SetTrigger("Jump");
            StartCoroutine(JumpCoroutine());  
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraTransform.Rotate(Vector3.up, mouseX);
    }

    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(jumpDelay);
        float elapsedTime = 0f;
        float initialY = transform.position.y;

        while (elapsedTime < jumpDuration)
        {
            float newY = Mathf.Lerp(initialY, initialY + jumpHeight, elapsedTime / jumpDuration);
            characterController.Move(new Vector3(0, newY - transform.position.y, 0)); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    [PunRPC]
    public void TakeDamage(float amount)
    {
        health -= amount;

        if (photonView.IsMine)
        {
            photonView.RPC("AdjustHealthBars", RpcTarget.All);
            if(health < 1)
                photonView.RPC("Die", RpcTarget.All);
        }      
    }

    [PunRPC]
    public void GetHeal(float amount)
    {
        health += amount;

        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("AdjustHealthBars", RpcTarget.All);
    }

    [PunRPC]
    public void AdjustHealthBars()
    {
        heathBarSelf.value = health;
        heathBarForOthers.value = health;
    }

    [PunRPC]
    public void Die()
    {
        isDead = true;

        if (animationManager != null)
        {
            animationManager.SetDeathStatus(true);
        }

        if(photonView.IsMine)
            gameManager.CheckAllPlayersDead();
    }

    [PunRPC]
    public void Revive()
    {
        isDead = false;
        health = 100f; 

        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("AdjustHealthBars", RpcTarget.All);

        if (animationManager != null)
        {
            animationManager.SetDeathStatus(false);
        }

        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<MagicBolt>(out MagicBolt magicBolt))
        {
            if (photonView.IsMine)
                GetComponent<PhotonView>().RPC("GetHeal", RpcTarget.All, magicBolt.healAmount);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent<ThirdPersonCharacterController>(out ThirdPersonCharacterController controller))
        {
            if (controller.isDead)
            {
                controller.helpPanel.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                    other.GetComponent<PhotonView>().RPC("Revive", RpcTarget.All);
            }        
        }
    }
    
}