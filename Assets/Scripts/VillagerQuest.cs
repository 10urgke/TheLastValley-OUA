using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class VillagerQuest : MonoBehaviourPun
{
    private Transform destination;
    private NavMeshAgent navMeshAgent;
    private AnimationManager animManager;
    public float speed;
    public float health;
    public Slider healthBar;
    public GameObject showHelpPanel;
    public bool isDying;
    private void Awake()
    {
        animManager = GetComponent<AnimationManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(photonView.IsMine)
            navMeshAgent.SetDestination(destination.position);
        healthBar.maxValue = health;
        healthBar.value = health;
        Dying();
    }
    [PunRPC]
    public void TakeDamage(float amount)
    {
        health -= amount;
        if(health < 0 && photonView.IsMine)
            photonView.RPC("Dying", RpcTarget.All);
        if (photonView.IsMine)
            photonView.RPC("UpdateHealthBar", RpcTarget.All);
    }
    [PunRPC]
    public void WalkToDest()
    {
        isDying = false;
        navMeshAgent.speed = speed;
        animManager.SetSpeed(0.5f);
        animManager.animator.SetBool("Dying", false);
        animManager.animator.SetBool("Walk", true);
    }
    [PunRPC]
    public void Dying()
    {
        isDying = true;
        navMeshAgent.speed = 0;
        animManager.SetSpeed(0);
        animManager.animator.SetBool("Dying", true);
        animManager.animator.SetBool("Walk", false);
    }
    [PunRPC]
    public void UpdateHealthBar()
    {
        healthBar.value = health;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<ThirdPersonCharacterController>() != null && isDying)
        {
            showHelpPanel.SetActive(true);
            if(Input.GetKeyDown(KeyCode.F) && photonView.IsMine)
                photonView.RPC("WalkToDest", RpcTarget.All);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ThirdPersonCharacterController>() != null && isDying)
        {
            showHelpPanel.SetActive(false);
        }
    }
}
