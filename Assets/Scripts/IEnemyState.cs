using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void Enter(Enemy enemy);
    void Execute();
}
public class IdleState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        Debug.Log("Enemy in idle");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0);
        this.enemy.navMeshAgent.speed = 0;
        this.enemy.target = null;
        this.enemy.Invoke("ChangeToWalkingState", 2f);
        
        
    }
    public void Execute()
    {
    }
}
public class WalkingState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        Debug.Log("Enemy in walking");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0.5f);
        this.enemy.navMeshAgent.speed = enemy.speed/2;
        this.enemy.SetDestination();
        
    }

    public void Execute()
    {
        if (enemy.navMeshAgent.remainingDistance < 1f)
            enemy.ChangeState(new IdleState());

    }
}
public class RunningState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        Debug.Log("Enemy in running");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(1f);
        this.enemy.navMeshAgent.speed = enemy.speed;
        this.enemy.SetDestination(enemy.target);
        
    }

    public void Execute()
    {
            if (enemy.target != null)
            {
                enemy.SetDestination(enemy.target);
                if (enemy.navMeshAgent.remainingDistance < enemy.attackRange)
                {
                    enemy.animManager.SetSpeed(0.1f);
                    enemy.navMeshAgent.speed = 0.1f;
                }
                else
                {
                    enemy.animManager.SetSpeed(1f);
                    enemy.navMeshAgent.speed = enemy.speed;
                }

                enemy.attackTimer += Time.deltaTime;
                //enemy.sliderUpdateTime += Time.deltaTime;
                enemy.attackCooldownBarSlider.value = enemy.attackTimer;
            }
            else
            {
                enemy.ChangeState(new IdleState());
            }
            if (enemy.navMeshAgent.remainingDistance < enemy.attackRange && enemy.attackTimer > enemy.attackCooldown)
            {
                if (!enemy.animManager.IsInState("GetHit"))
                    enemy.ChangeState(new AttackingState());

            }
            


    }

}
public class AttackingState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        Debug.Log("Enemy is attacking");
        this.enemy = enemy;
        enemy.Attack();
        
    }

    public void Execute()
    {

        if (enemy.target != null)
        {
            enemy.ChangeState(new RunningState());
        }
        else
            enemy.ChangeState(new IdleState());
   
    }
}
public class DyingState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        Debug.Log("Enemy is dying");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0f);
        this.enemy.navMeshAgent.speed = 0f;
        this.enemy.GetComponent<PhotonView>().RPC("Death", RpcTarget.All);
        
    }

    public void Execute()
    {
    }
}
