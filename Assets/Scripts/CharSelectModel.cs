using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectModel : MonoBehaviour
{
    private Animator charAnimator;
    public ParticleSystem fx;
    void Start()
    {
        charAnimator = GetComponent<Animator>();
    }

    public void MouseOverUI()
    {
        charAnimator.SetBool("OnHover", true);
        fx.Play();
    }

    public void MouseNotOverUI()
    {
        charAnimator.SetBool("OnHover", false);
        fx.Stop();
    }
}
