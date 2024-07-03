using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTestScript : MonoBehaviour
{
    [SerializeField]Animator animator;
    float speed;
    void Anim(string animName)
    {
        animator.CrossFade(Animator.StringToHash(animName), 0.1f);
    }
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
        Debug.Log(forwardPressed);
        if (forwardPressed&&speed<=0.5f)
        {
            speed += Time.deltaTime * 2;
            animator.SetFloat("speed", speed);
            animator.SetBool("Walk", true);
        }
        if (forwardPressed && runPressed && speed <= 1)
        {
            speed += Time.deltaTime * 2;
            animator.SetFloat("speed", speed);
            animator.SetBool("Walk", true);
        }
        if (!forwardPressed&& speed>=0 || !runPressed && speed >= 0.5F)
        {
            speed -= Time.deltaTime * 2;
            animator.SetFloat("speed", speed);
        }   
        if (Input.GetKey(KeyCode.Space))
        {
            Anim("Jump");
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Anim("Attack");
        }
        if (Input.GetKey(KeyCode.D))
        {
            Anim("Death");
        }
        if (Input.GetKey(KeyCode.O))
        {
            Anim("WarriorDeath");
        }
    }
}
