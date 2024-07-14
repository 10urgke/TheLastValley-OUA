using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviourPun
{
    //turn protected after tests
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
    public bool IsInState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    //triggers/ death,gethit,attack..

    public virtual void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);

        //if(photonView.IsMine)
        //    photonView.RPC("SetTriggerRPC", RpcTarget.Others, triggerName);
    }
}
