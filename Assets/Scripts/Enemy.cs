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
    [SerializeField] private Slider healthBarSlider;
    public Slider attackCooldownBarSlider;
    public float attackCooldown;
    public float attackTimer;
    //[SerializeField] private float networkAttackSliderUpdateTime = 0.2f;
    //public float sliderUpdateTime;
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
        //sliderUpdateTime = 0;
        Debug.Log("enemy created");
    }
    private void Update()
    {
        currentState.Execute();

        //if(sliderUpdateTime > networkAttackSliderUpdateTime)
        //{
        //    sliderUpdateTime = 0;
        //    GetComponent<PhotonView>().RPC("UpdateAttackTimerOnNetwork", RpcTarget.All);
        //}
        //test for taking damage over the network
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 10f);
        }
            
        

            
    }
    public void ChangeState(IEnemyState newState)
    {
        currentState = newState;
        currentState.Enter(this);
    }
    //for invoke in idlestate
    public void ChangeToWalkingState()
    {
        ChangeState(new WalkingState());
    }
    [PunRPC]
    public void TakeDamage(float amount)
    {
        Debug.Log("Take damage func");
        health -= amount;
        healthBarSlider.value = health;
        if(health <= 0 )
            ChangeState(new DyingState());
        else
        {
            if(!animManager.IsInState("GetHit"))
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
    [PunRPC]
    public void Death()
    {
        animManager.SetTrigger("Death");
        Debug.Log("Death func");
        //pool for different areas
        Destroy(gameObject, 5f);
    }
    [PunRPC]
    public void Attack()
    {
        animManager.SetTrigger("Attack");
        Debug.Log("Attack func");
        if (photonView.IsMine)
            GetComponent<PhotonView>().RPC("UpdateAttackTimerOnNetwork", RpcTarget.Others);
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;
        //Debug.Log("attack");
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
        ChangeState(new RunningState());
    }
    public void LeaveTarget()
    {
        target = null;
        ChangeState(new IdleState());
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

