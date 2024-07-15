using Photon.Pun;
using UnityEngine;

public interface IEnemyState
{
    void Enter(Enemy enemy);
    void Execute();
    void Exit();
}
public class IdleState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy in idle");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0);
        this.enemy.navMeshAgent.speed = 0;
        this.enemy.target = null;
        this.enemy.StartCoroutine(enemy.ChangeStateAfterTime("WalkingState",2f));
        
        
    }
    public void Execute()
    {
    }

    public void Exit()
    {
        enemy.StopAllCoroutines();
    }
}
public class WalkingState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy in walking");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0.5f);
        this.enemy.navMeshAgent.speed = enemy.speed/2;
        this.enemy.SetDestination();
        
    }

    public void Execute()
    {
        if (enemy.navMeshAgent.remainingDistance < 1f && enemy.photonView.IsMine)
            enemy.GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "IdleState");

    }
    public void Exit()
    {
        //enemy.StopAllCoroutines();
    }
}
public class RunningState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy in running");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(1f);
        this.enemy.navMeshAgent.speed = enemy.speed;

        if (enemy.photonView.IsMine)
        {
            this.enemy.SetDestination(this.enemy.target);
        }
            
        
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
                enemy.attackCooldownBarSlider.value = enemy.attackTimer;
            }
            else if(enemy.photonView.IsMine)
            {
                enemy.GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "IdleState");
            }
            if (enemy.navMeshAgent.remainingDistance < enemy.attackRange && enemy.attackTimer > enemy.attackCooldown)
            {
                if (!enemy.animManager.IsInState("GetHit") || !enemy.animManager.IsInState("Attack"))
                    if(enemy.photonView.IsMine)
                        enemy.GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "AttackingState");

            }
            


    }
    public void Exit()
    {
        //enemy.StopAllCoroutines();
    }

}
public class AttackingState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.Attack();
    }

    public void Execute()
    {

        if (enemy.target != null && enemy.photonView.IsMine)
        {
            enemy.GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "RunningState");
        }
        else if (enemy.photonView.IsMine)
            enemy.GetComponent<PhotonView>().RPC("ChangeStateRPC", RpcTarget.All, "IdleState");

    }
    public void Exit()
    {
        //enemy.StopAllCoroutines();
    }
}
public class DyingState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy is dying");
        this.enemy = enemy;
        this.enemy.animManager.SetSpeed(0f);
        this.enemy.navMeshAgent.speed = 0f;
        this.enemy.healthBarSlider.gameObject.SetActive(false);
        this.enemy.attackCooldownBarSlider.gameObject.SetActive(false);
        this.enemy.Death();

        
    }

    public void Execute()
    {
    }
    public void Exit()
    {
        //enemy.StopAllCoroutines();
    }
}
