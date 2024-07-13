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


    //triggers/ death,gethit,attack..

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
