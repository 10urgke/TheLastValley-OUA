using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimTestScript : MonoBehaviour
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
            animator.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("meleeAttack");
            animator.SetTrigger("Attack1");
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetTrigger("death");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetTrigger("death2");
        }if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetTrigger("gethit");
        }
    }
}
