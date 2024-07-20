
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("Make deactive on prefab")]
    public GameObject camOne;
    public GameObject camTwo;
    public ThirdPersonCharacterController characterController;
    public GameObject canvasForYourself;
    //public AnimTestScript animTest;
    [Header("Make active on prefab")]
    public GameObject canvasForOthers;


    public void IsLocalPlayer(bool isActive)
    {
        //Deactivated in the editor
        camOne.SetActive(isActive);
        camTwo.SetActive(isActive);
        characterController.enabled = isActive;
        canvasForYourself.SetActive(isActive);
        //animTest.enabled = isActive;

        //Activated in the editor
        canvasForOthers.SetActive(!isActive);
}
}
