using Photon.Pun;
using UnityEngine;

public class AnimationManager : MonoBehaviourPun
{
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

    public float InStateTime(string stateName)
    {
        if (IsInState(stateName))
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        else
        {
            return -1f;
        }
    }

    //triggers/ death,gethit,attack.. for npcs

    public virtual void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);

        //if(photonView.IsMine)
        //    photonView.RPC("SetTriggerRPC", RpcTarget.Others, triggerName);
    }
}
