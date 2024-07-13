using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviourPun
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //booleans
    public void Walk(bool isWalking)
    {
        animator.SetBool("Walk", isWalking);
    }
    public void Run(bool isRunning)
    {
        animator.SetBool("Run", isRunning);
    }


        //triggers/ death,gethit

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
