using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public GameObject camOne;
    public GameObject camTwo;
    public ThirdPersonCharacterController characterController;
    public AnimTestScript animTest;


    public void IsLocalPlayer(bool isActive)
    {
            camOne.SetActive(isActive);
            camTwo.SetActive(isActive);
            characterController.enabled = isActive;
            animTest.enabled = isActive;
    }
}
