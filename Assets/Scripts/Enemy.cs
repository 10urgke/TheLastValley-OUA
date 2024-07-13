using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private AnimationManager animManager;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject target;
    [SerializeField] private List<Transform> destinations;
    private void Awake()
    {
        animManager = GetComponent<AnimationManager>();
        healthBarSlider.value = health;
        healthBarSlider.maxValue = health;
        healthBarSlider.minValue = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetDestination();
    }
    private void Update()
    {
        if(target != null)
            SetDestination(target);
        if (Input.GetKeyDown(KeyCode.K))           
            TakeDamage(10f);

            
    }
    public void TakeDamage(float amount)
    {
        animManager.SetTrigger("GetHit");
        health -= amount;
        healthBarSlider.value = health;
        if(health <= 0 )
            Death();
    }
    
    public void Death()
    {
        animManager.SetTrigger("Death");
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
            target = null;
            SetDestination();
        }
    }
    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
    }
    public void SetDestination(GameObject target)
    {
        navMeshAgent.SetDestination(target.transform.position);
    }
    public void SetDestination()
    {
        if (destinations.Count <= 0)
            FindDestinations();
        else
            navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);

        
    }
    public void FindDestinations()
    {
        var dests = GameObject.Find("EnemyDest");
        foreach (Transform dest in dests.GetComponentInChildren<Transform>())
        {
            destinations.Add(dest);
        }
        navMeshAgent.SetDestination(destinations[Random.Range(0, destinations.Count)].position);

    }
}
