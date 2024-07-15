using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : AnimationManager
{
    public void SetWalkStatus(bool isActive)
    {
        animator.SetBool("Walk", isActive);
    }
    public void SetSecondStatus(bool isActive)
    {
        animator.SetBool("Second", isActive);
    }
    public void SetCarryStatus(bool isActive)
    {
        animator.SetBool("Carry", isActive);
    }


    [PunRPC]
    public void SetTriggerRPC(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public override void SetTrigger(string triggerName)
    {
        base.SetTrigger(triggerName);

        if (photonView.IsMine)
            photonView.RPC("SetTriggerRPC", RpcTarget.Others, triggerName);
    }
}
