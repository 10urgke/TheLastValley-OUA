using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTestScript : MonoBehaviourPun
{
    [SerializeField]Animator animator;
    float speed;
    private void Start()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        //if(Input.GetKey(KeyCode.W)) 
        {
            Debug.Log("sda");
            animator.SetBool("walk", true);
            if(Input.GetKey(KeyCode.C)) 
            {
                animator.SetBool("walk", false);
                animator.SetBool("aimwalk", true);
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                animator.SetBool("aimwalk", false);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("walk", false);
                animator.SetBool("aimwalk", false);
                animator.SetBool("run", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animator.SetBool("run", false);
            }
        }
        if (forwardPressed&&speed<=0.5f || forwardPressed && runPressed && speed <= 1 || forwardPressed && animator.GetBool("Walk")==false && speed <= 1)
        {
            speed += Time.deltaTime * 2;
            animator.SetFloat("speed", speed);
            //animator.SetBool("Walk", true);
        }
        if (!forwardPressed&& speed>=0 || !runPressed && speed >= 0.5F && animator.GetBool("Walk") == true)
        {
            Debug.Log("giriyor");
            speed -= Time.deltaTime * 2;
            animator.SetFloat("speed", speed);
        }   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Anim("Jump");
            SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetTrigger("meleeAttack");
            SetTrigger("Attack1");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("CarryWalk", false);
            animator.SetBool("AimWalk", true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animator.SetBool("Walk", true);
            animator.SetBool("CarryWalk", false);
            animator.SetBool("AimWalk", false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("CarryWalk", true);
            animator.SetBool("AimWalk", false);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            animator.SetBool("Walk", true);
            animator.SetBool("CarryWalk", false);
            animator.SetBool("AimWalk", false);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetTrigger("death");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetTrigger("death2");
        }if (Input.GetKeyDown(KeyCode.G))
        {
            SetTrigger("gethit");
        }
        
    }
    [PunRPC]
    public void SetTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);

        photonView.RPC("SetTriggerRPC", RpcTarget.Others, triggerName);
    }
}
