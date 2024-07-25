using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviourPun
{
    [SerializeField] private List<Transform> destinations;
    private NavMeshAgent navMeshAgent;
    private AnimationManager animManager;
    public float speed;
    public int idleTimeMin;
    public int idleTimeMax;
    public float idleTime;
    public bool isIdle;

    private void Awake()
    {
        animManager = GetComponent<AnimationManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animManager.SetSpeed(0f);
        navMeshAgent.speed = 0;
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
        StartCoroutine(WaitInIdle());       
    }

    private void Update()
    {
        if(navMeshAgent.remainingDistance < 2f && !isIdle)
        {
            StartCoroutine(WaitInIdle());
        }
    }

    public IEnumerator WaitInIdle()
    {
        if(!isIdle)
        {
            animManager.SetSpeed(0f);
            navMeshAgent.speed = 0;
            isIdle = true;
        }    
        
        yield return new WaitForSeconds(idleTime);
        SetDestination();
    }
    public void SetDestination()
    {
        if (photonView.IsMine && destinations.Count > 0)
        {
            animManager.SetSpeed(0.5f);
            navMeshAgent.speed = speed;
            navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);
            idleTime = Random.Range(idleTimeMin, idleTimeMax);
            isIdle = false;
        }
    }
}

