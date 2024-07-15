using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTestScript : MonoBehaviourPun
{
    PlayerAnimationManager animManager;
    float speed;
    private void Start()
    {
        animManager = GetComponent<PlayerAnimationManager>();


    }
    private void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        //if(Input.GetKey(KeyCode.W)) 
        animManager.SetWalkStatus(true);
        if(Input.GetKey(KeyCode.C)) 
        {
            animManager.SetWalkStatus(false);
            animManager.SetSecondStatus(true);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            animManager.SetSecondStatus(false);
        }
        if (forwardPressed&&speed<=0.5f || forwardPressed && runPressed && speed <= 1 || forwardPressed && animManager.animator.GetBool("Walk")==false && speed <= 1)
        {
            speed += Time.deltaTime * 2;
            animManager.SetSpeed(speed);
        }
        if (!forwardPressed&& speed>=0 || !runPressed && speed >= 0.5F && animManager.animator.GetBool("Walk") == true)
        {
            speed -= Time.deltaTime * 2;
            animManager.SetSpeed(speed);
        }   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Anim("Jump");
            animManager.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animManager.SetTrigger("meleeAttack");
            animManager.SetTrigger("Attack1");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            animManager.SetWalkStatus(false);
            animManager.SetCarryStatus(false);
            animManager.SetSecondStatus(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animManager.SetWalkStatus(true);
            animManager.SetCarryStatus(false);
            animManager.SetSecondStatus(false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animManager.SetWalkStatus(false);
            animManager.SetCarryStatus(true);
            animManager.SetSecondStatus(false);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            animManager.SetWalkStatus(true);
            animManager.SetCarryStatus(false);
            animManager.SetSecondStatus(false);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            animManager.SetTrigger("death");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animManager.SetTrigger("death2");
        }if (Input.GetKeyDown(KeyCode.G))
        {
            animManager.SetTrigger("gethit");
        }
        
    }
}
