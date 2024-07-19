using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ArcherController : ThirdPersonCharacterController
{
    [Header("Archer Settings")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float arrowForce = 2000f;
    public float arrowDamage = 10f;
    public int projectilePoolSize = 10;
    public float waitTimeForShot = 0.9f;
    public float waitTimeForNextAttackAfterShot = 0.9f;
    public CinemachineFreeLook freeLookCam;
    public float freeLookZoom = 20;
    public ObjectPooler pooler;

    private bool isShooting;

    protected override void Start()
    {
        base.Start();
        freeLookCam = GetComponent<PlayerSetup>().camTwo.GetComponent<CinemachineFreeLook>();
        pooler = GetComponent<ObjectPooler>();
        pooler.MakePool(arrowPrefab, projectilePoolSize);
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
                StartCoroutine(SecondShotCoroutine());
            }
            else
            {
                animationManager.SetTrigger("FullShot");
                isShooting = true;
                StartCoroutine(RegularShotCoroutine());
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
            freeLookCam.m_Lens.FieldOfView = freeLookZoom;
            animationManager.SetWalkStatus(false);
            animationManager.SetSecondStatus(true);
            animationManager.SetCarryStatus(false);

        }
        if (Input.GetButtonUp("Fire2"))
        {
            animationManager.SetWalkStatus(true);
            animationManager.SetSecondStatus(false);
            animationManager.SetCarryStatus(false);
            freeLookCam.m_Lens.FieldOfView = 40;
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

    private IEnumerator RegularShotCoroutine()
    {
        yield return new WaitForSeconds(waitTimeForShot);
        ShootArrow();
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShot);
        isShooting = false;
    }
    private IEnumerator SecondShotCoroutine()
    {
        ShootArrow();
        yield return new WaitForSeconds(waitTimeForNextAttackAfterShot);
        isShooting = false;
    }

    private void ShootArrow()
    {
        //pool
        GameObject arrow = pooler.GetPooledObject(arrowPrefab);
        arrow.transform.position = arrowSpawnPoint.position + transform.forward;
        arrow.transform.rotation = arrowSpawnPoint.rotation * Quaternion.Euler(0, 90, 0);
        arrow.GetComponent<Arrow>().damage = arrowDamage;
        arrow.SetActive(true);
        arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * arrowForce);


        //GameObject newArrow = PhotonNetwork.Instantiate(arrowPrefab.name, arrowSpawnPoint.position + transform.forward, arrowSpawnPoint.rotation * Quaternion.Euler(0, 90, 0));
        //newArrow.GetComponent<Rigidbody>().AddForce(transform.forward * arrowForce);
    }
}
