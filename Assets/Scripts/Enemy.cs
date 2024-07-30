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

    public bool isDying = false;

    //for villager quest
    public VillagerQuest villager;

    //for spawn quest
    public SpawnQuestManager spawnQuest;

    //for management enemy camps
    public EnemyManager enemyManager;

    public bool isBoos;

    private void Awake()
    {
        animManager = GetComponent<AnimationManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
        healthBarSlider.minValue = 0;

        attackCooldownBarSlider.maxValue = attackCooldown;
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;

        if (animManager != null && animManager.animator == null)
        {
            animManager.animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {        
        if(villager == null)
            ChangeState(new IdleState());

        else
        {
            target = villager.gameObject;
            ChangeState(new RunningState());
        }
    }

    private void OnEnable()
    {
        health = healthBarSlider.maxValue;
        healthBarSlider.value = health;

        if (!healthBarSlider.gameObject.activeInHierarchy || !attackCooldownBarSlider.gameObject.activeInHierarchy)
        {
            healthBarSlider.gameObject.SetActive(true);
            attackCooldownBarSlider.gameObject.SetActive(true);
        }

        if (villager == null)
        {
            target = null;
            ChangeState(new IdleState());
        }          

        else
        {                                  
            target = villager.gameObject;
            ChangeState(new RunningState());
        }
    }

    private void Update()
    {
        currentState.Execute();   
    }

    #region states
    public void ChangeState(IEnemyState newState)
    {
        if(isDying)
            return;

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
        health -= amount;
        healthBarSlider.value = health;

        if(health <= 0)
        {
            if (photonView.IsMine && !isBoos)
                GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "DyingState");
            else if (isBoos)
            {
                Cursor.lockState = CursorLockMode.Confined;
                PhotonNetwork.LoadLevel("Success");
            }
                
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
    }

    public void Death()
    {
        animManager.SetTrigger("Death");

        if(photonView.IsMine)
        {
            if (villager != null || enemyManager != null)
            {
                StartCoroutine(FakeDeathAfterSecsOnNetwork(5f));
            }

            else
                StartCoroutine(DeathAfterSecsOnNetwork(5f));
        }          
    }

    public IEnumerator DeathAfterSecsOnNetwork(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<PhotonView>().RPC("DeathOnNetwork", RpcTarget.All);
    }
    public IEnumerator FakeDeathAfterSecsOnNetwork(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<PhotonView>().RPC("FakeDeathOnNetwork", RpcTarget.All);
    }

    [PunRPC]
    public void DeathOnNetwork()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
            
    }
    [PunRPC]
    public void FakeDeathOnNetwork()
    {
        if (enemyManager != null)
            enemyManager.EnemyDeath(gameObject);

        isDying = false;
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void Attack()
    {
        animManager.SetTrigger("Attack");  

        if (photonView.IsMine)
        {
            target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            GetComponent<PhotonView>().RPC("UpdateAttackTimerOnNetwork", RpcTarget.Others);         
        }
            
        attackTimer = 0;
        attackCooldownBarSlider.value = attackTimer;

        if(villager != null)
        {
            if (target == villager.gameObject && villager.isDying == true)
                target = null;
        }      
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(target == null)
        {
            if (other.GetComponent<ThirdPersonCharacterController>() != null)
            {
                SetTarget(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null && !(currentState is DyingState))
        {
            if (other.GetComponent<ThirdPersonCharacterController>() != null)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.TryGetComponent<Arrow>(out Arrow arrowProjectile))
            {         
                GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, arrowProjectile.damage);
            }

            else if (collision.gameObject.TryGetComponent<MagicBolt>(out MagicBolt magicProjectile))
            {
                GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, magicProjectile.damage);
            }

            else if (collision.gameObject.TryGetComponent<Sword>(out Sword sword))
            {
                GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, sword.damage);
            }
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
        //For fix navmesh glich, only the master client will set destination 
        if (photonView.IsMine)
            navMeshAgent.SetDestination(target.transform.position);
    }

    public void SetDestination()
    {
        if (photonView.IsMine)
        {
            if (destinations.Count > 0)
            {
                navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);
            }

            else
            {
                var dests = gameObject.transform.parent.GetChild(0);
                foreach (Transform dest in dests.GetComponentInChildren<Transform>())
                {
                    destinations.Add(dest);
                }
                navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);
            }
        }         
    }
}

