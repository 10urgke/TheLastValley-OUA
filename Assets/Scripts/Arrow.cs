using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Arrow : MonoBehaviourPun
{
    public float lifeTime = 5f;
    public float damage = 10f;

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
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
