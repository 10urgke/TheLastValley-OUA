using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class VillagerQuest : MonoBehaviourPun
{
    public Transform destination;
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
    private void Update()
    {
        if(navMeshAgent.remainingDistance < 1f && photonView.IsMine)
            photonView.RPC("QuestFinished", RpcTarget.All);
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
        showHelpPanel.SetActive(false);
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
    [PunRPC]
    public void GetHeal(float amount)
    {
        health += amount;
        if (photonView.IsMine)
            photonView.RPC("UpdateHealthBar", RpcTarget.All);
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<MagicBolt>(out MagicBolt magicBolt))
        {
            if (photonView.IsMine)
                photonView.RPC("GetHeal", RpcTarget.All, magicBolt.healAmount);
        }
    }
    [PunRPC]
    public void QuestFinished()
    {
        //disable or delete quest, maybe show fx
        Destroy(transform.parent.gameObject);
    }
}
