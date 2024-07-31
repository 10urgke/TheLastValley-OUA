using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBolt : MonoBehaviourPun
{
    public float lifeTime = 5f;
    public float damage = 10f;
    public float healAmount = 10f;
    public ParticleSystem explosionFx;
    public float explosionTime = 1f;
    private Rigidbody rb;
    public GameObject mesh;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        OnSpawn();
    }

    private IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ExplosionCor());
    }

    private IEnumerator ExplosionCor()
    {
        if(explosionFx.isStopped)
        {
            OnHit();
        }     

        yield return new WaitForSeconds(explosionTime);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    public void OnHit()
    {
        explosionFx.Play();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        mesh.SetActive(false);
    }
    public void OnSpawn()
    {
        StartCoroutine(ReturnToPoolAfterTime());
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        mesh.SetActive(true);
    }

}
