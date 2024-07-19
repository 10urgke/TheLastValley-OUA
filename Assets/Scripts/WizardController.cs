using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WizardController : ThirdPersonCharacterController
{
    [Header("Wizard Settings")]
    public GameObject magicPrefab;
    public Transform magicSpawnPoint;
    public float magicForce = 2000f;
    public float waitTimeForShot = 0.9f;
    public float waitTimeForNextAttackAfterShot = 0.9f;

    private bool isShooting;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        HandleSecondStatus();
        HandleCarryStatus();
        if (Input.GetButtonDown("Fire1") && !isShooting)
        {
            if (animationManager.animator.GetBool("Carry"))
                return;
            else if (animationManager.animator.GetBool("Second"))
            {
                animationManager.SetTrigger("Recoil");
                isShooting = true;
                StartCoroutine(CastHealCoroutine());
            }
            else
            {
                animationManager.SetTrigger("FullShot");
                isShooting = true;
                StartCoroutine(RegularAttackCoroutine());
            }        
        }
    }
    private void HandleSecondStatus()
    {
        if (animationManager.animator.GetBool("Carry"))
            return;
        if (Input.GetButtonDown("Fire2"))
        {
            sprintBlock = true;
            animationManager.SetWalkStatus(false);
            animationManager.SetSecondStatus(true);
            animationManager.SetCarryStatus(false);

        }
        if (Input.GetButtonUp("Fire2"))
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(false);
            sprintBlock = false;
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
        GameObject newArrow = PhotonNetwork.Instantiate(magicPrefab.name, magicSpawnPoint.position + transform.forward, magicSpawnPoint.rotation * Quaternion.Euler(0, 90, 0));
        newArrow.GetComponent<Rigidbody>().AddForce(transform.forward * magicForce);
    }
    public void Heal()
    {

    }
}
