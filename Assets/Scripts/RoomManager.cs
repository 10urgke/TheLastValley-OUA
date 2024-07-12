using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;
    void Start()
    {
        Debug.Log("Connecting..");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connected to server");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("test",null,null);
        Debug.Log("in lobby");

        
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("in room");
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer(true);
    }
}
