using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class WizardController : ThirdPersonCharacterController
{
    [Header("Wizard Settings")]
    public GameObject magicPrefab;
    public int projectilePoolSize = 10;
    public Transform magicSpawnPoint;
    public float magicForce = 2000f;
    public float magicDamage = 10f;
    public float magicHeal = 10f;
    public float waitTimeForShoot = 0.9f;
    public float waitTimeForNextAttackAfterShoot = 0.9f;
    public ParticleSystem attackFx;
    public ParticleSystem healFx;

    private bool isShooting;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        HandleCarryStatus();

        if (Input.GetButtonDown("Fire1") && !isShooting)
        {
            if (animationManager.animator.GetBool("Carry"))
                return;

            animationManager.SetTrigger("Attack");
            isShooting = true;
            StartCoroutine(RegularAttackCoroutine());      
        }

        if (Input.GetButtonDown("Fire2") && !isShooting)
        {
            animationManager.SetTrigger("Cast");           
            isShooting = true;
            StartCoroutine(CastHealCoroutine());
        }
    }

    private void HandleCarryStatus()
    {
        if (isCarrying)
        {
            sprintBlock = true;
            animationManager.SetWalkStatus(false);
            animationManager.SetCarryStatus(true);

        }

        else if (!isCarrying)
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetCarryStatus(false);
            sprintBlock = false;
        }
    }

    private IEnumerator RegularAttackCoroutine()
    {          
        yield return new WaitForSeconds(waitTimeForShoot);
        SpawnMagic();
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShoot);
        isShooting = false;
    }
    private IEnumerator CastHealCoroutine()
    {
        GetSelfHeal(magicHeal);
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShoot);
        isShooting = false;
    }

    private void SpawnMagic()
    {
        if(photonView.IsMine)
            photonView.RPC("PlayAttackFx", RpcTarget.All);

        GameObject magicMissile = PhotonNetwork.Instantiate(magicPrefab.name, magicSpawnPoint.position + transform.forward, magicSpawnPoint.rotation * Quaternion.Euler(0, 90, 0));
        magicMissile.GetComponent<Rigidbody>().AddForce(transform.forward * magicForce);
        magicMissile.GetComponent<MagicBolt>().damage = magicDamage;
        magicMissile.GetComponent<MagicBolt>().healAmount = magicHeal;
    }
    public void GetSelfHeal(float amount)
    {
        if (photonView.IsMine)
            photonView.RPC("PlayHealFx", RpcTarget.All);

        photonView.RPC("GetHeal", RpcTarget.All, amount);      
    }

    [PunRPC]
    public void PlayAttackFx()
    {
        attackFx.Play();
    }

    [PunRPC]
    public void PlayHealFx()
    {
        healFx.Play();
    }
}
