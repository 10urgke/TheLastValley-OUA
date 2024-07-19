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
    public float waitTimeForShot = 0.9f;
    public float waitTimeForNextAttackAfterShot = 0.9f;
    public ObjectPooler pooler;

    private bool isShooting;

    protected override void Start()
    {
        base.Start();
        pooler = GetComponent<ObjectPooler>();
        pooler.MakePool(magicPrefab, projectilePoolSize);
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
        if (animationManager.animator.GetBool("Second"))
            return;
        if (isCarrying)
        {
            sprintBlock = true;
            animationManager.SetWalkStatus(false);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(true);

        }
        else if (!isCarrying)
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(false);
            sprintBlock = false;
        }
    }

    private IEnumerator RegularAttackCoroutine()
    {
        yield return new WaitForSeconds(waitTimeForShot);
        SpawnMagic();
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShot);
        isShooting = false;
    }
    private IEnumerator CastHealCoroutine()
    {
        Heal();
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShot);
        isShooting = false;
    }

    private void SpawnMagic()
    {
        //pool
        GameObject magic = pooler.GetPooledObject(magicPrefab);
        magic.transform.position = magicSpawnPoint.position + transform.forward;
        magic.transform.rotation = magicSpawnPoint.rotation * Quaternion.Euler(0, 90, 0);
        magic.SetActive(true);
        magic.GetComponent<Rigidbody>().velocity = Vector3.zero;
        magic.GetComponent<Rigidbody>().AddForce(transform.forward * magicForce);

        //GameObject magicMissile = PhotonNetwork.Instantiate(magicPrefab.name, magicSpawnPoint.position + transform.forward, magicSpawnPoint.rotation * Quaternion.Euler(0, 90, 0));
        //magicMissile.GetComponent<Rigidbody>().AddForce(transform.forward * magicForce);
    }
    public void Heal()
    {

    }
}
