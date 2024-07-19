using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 10f;
    public float healAmount = 10f;

    private void OnEnable()
    {
        StartCoroutine(ReturnToPoolAfterTime());
    }

    private IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }
    private void OnCollisionEnter(Collision collision)
    {

        //check player and enemy
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
