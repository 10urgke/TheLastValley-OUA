using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WariorController : ThirdPersonCharacterController
{
    [Header("Warior Settings")]
    public float waitTimeForAttack = 0.9f;
    public float waitTimeForNextAttack = 1f;

    private bool isAttacking;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        HandleSecondStatus();
        HandleCarryStatus();
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            if (animationManager.animator.GetBool("Carry"))
                return;
            else if (animationManager.animator.GetBool("Second"))
            {
                animationManager.SetTrigger("Attack2");
                isAttacking = true;
                StartCoroutine(SecondAttackCoroutine());
            }
            else
            {
                animationManager.SetTrigger("Attack1");
                isAttacking = true;
                StartCoroutine(RegularAttackCoroutine());
            }        
        }
    }
    private void HandleSecondStatus()
    {
        if (animationManager.animator.GetBool("Carry"))
            return;
        if (Input.GetButtonDown("Fire2"))
        {
            sprintBlock = true;
            animationManager.SetWalkStatus(false);
            animationManager.SetSecondStatus(true);
            animationManager.SetCarryStatus(false);

        }
        if (Input.GetButtonUp("Fire2"))
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(false);
            sprintBlock = false;
        }
    }
    private void HandleCarryStatus()
    {
        if (animationManager.animator.GetBool("Second"))
            return;
        if (isCarrying)
        {
            sprintBlock = true;
            animationManager.SetWalkStatus(false);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(true);

        }
        else if (!isCarrying)
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(false);
            sprintBlock = false;
        }
    }

    private IEnumerator RegularAttackCoroutine()
    {
        yield return new WaitForSeconds(waitTimeForAttack + waitTimeForNextAttack);
        isAttacking = false;
    }
    private IEnumerator SecondAttackCoroutine()
    {
        yield return new WaitForSeconds(waitTimeForNextAttack);
        isAttacking = false;
    }
}
