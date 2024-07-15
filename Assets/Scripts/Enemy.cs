using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    [SerializeField] private float health;
    [SerializeField] private float damage;  
    public Slider healthBarSlider;
    public Slider attackCooldownBarSlider;
    public float attackCooldown;
    public float attackTimer;
    public float speed = 3;
    public AnimationManager animManager;
    public NavMeshAgent navMeshAgent;
    public GameObject target;
    public float attackRange = 1f;
    [SerializeField] private List<Transform> destinations;

    [SerializeField] private IEnemyState currentState;
    private void Start()
    {
        animManager = GetComponent<AnimationManager>();
        healthBarSlider.value = health;
        healthBarSlider.maxValue = health;
        healthBarSlider.minValue = 0;
        attackCooldownBarSlider.maxValue = attackCooldown;
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;
        navMeshAgent = GetComponent<NavMeshAgent>();
        ChangeState(new IdleState());
        //Debug.Log("enemy created");
    }
    private void Update()
    {
        currentState.Execute();

        //test for taking damage over the network
        if (Input.GetKeyDown(KeyCode.K) && photonView.IsMine)
        {
            GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 10f);
        }
            
        

            
    }
    #region states
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        currentState.Enter(this);
    }
    [PunRPC]
    public void ChangeStateRPC(string stateName)
    {
        switch(stateName)
        {
            case "IdleState":
                ChangeState(new IdleState());
                break;
            case "WalkingState":
                ChangeState(new WalkingState());
                break;
            case "RunningState":
                ChangeState(new RunningState());
                break;
            case "AttackingState":
                ChangeState(new AttackingState());
                break;
            case "DyingState":
                ChangeState(new DyingState());
                break;
            default:
                Debug.LogError("Check Name");
                break;
        }
            
    }
    public IEnumerator ChangeStateAfterTime(string stateName, float time)
    {
        yield return new WaitForSeconds(time);
        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, stateName);
    }
    
    #endregion
    [PunRPC]
    public void TakeDamage(float amount)
    {
        Debug.Log("Take damage func");
        health -= amount;
        healthBarSlider.value = health;
        if(health <= 0)
        {
            if (photonView.IsMine)
                GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "DyingState");
        }   
        else
        {
            if(!animManager.IsInState("GetHit") || !animManager.IsInState("Attack"))
                animManager.SetTrigger("GetHit");                  
        }
            
    }
    [PunRPC]
    public void UpdateAttackTimerOnNetwork()
    {
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;
        Debug.Log("update attack slider func");
    }
    public void Death()
    {
        animManager.SetTrigger("Death");
        //Debug.Log("Death func");
        //pool for different areas
        Destroy(gameObject, 5f);
        //if (photonView.IsMine)
        //    StartCoroutine(DeathAfterSecsOnNetwork(5f));
    }

    //for networked instantiated objects
    public IEnumerator DeathAfterSecsOnNetwork(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<PhotonView>().RPC("DeathOnNetwork", RpcTarget.All);
    }
    [PunRPC]
    public void DeathOnNetwork()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void Attack()
    {
        animManager.SetTrigger("Attack");
        //Debug.Log("Attack to " + target.name);
        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("UpdateAttackTimerOnNetwork", RpcTarget.Others);
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(target == null)
        {
            if (other.GetComponent<PlayerAnimationManager>() != null)
            {
                SetTarget(other.gameObject);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(target != null &&  other.gameObject == target)
        {
            LeaveTarget();
        }
    }
    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "RunningState");
    }
    public void LeaveTarget()
    {
        target = null;
        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "IdleState");
    }
    public void SetDestination(GameObject target)
    {
        navMeshAgent.SetDestination(target.transform.position);
    }
    public void SetDestination()
    {
        if (destinations.Count > 0)
        {
            navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);
        }
        else
        {

            //might will be spawned in a game object and take its destination locs?
            //test for instantiate
            var dests = GameObject.Find("EnemyDest");
            foreach (Transform dest in dests.GetComponentInChildren<Transform>())
            {
                destinations.Add(dest);
            }
            navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);
        }
    }

}

