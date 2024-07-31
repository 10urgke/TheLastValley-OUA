using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform spawnPoint;
    public int reSpawnTime = 2;
    public void EnemyDeath(GameObject enemy)
    {
        Debug.Log("EnemyDeathCalled");
        StartCoroutine(ReSpawnEnemy(enemy));
    }
    public IEnumerator ReSpawnEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(reSpawnTime);
        Spawn(enemy);
    }
    public void Spawn(GameObject enemy)
    {
        enemy.transform.position = spawnPoint.position;
        enemy.gameObject.SetActive(true);
    }
}
