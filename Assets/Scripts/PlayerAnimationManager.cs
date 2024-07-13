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
}
