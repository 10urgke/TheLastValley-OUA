using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player;
    [SerializeField] private string roomName = "test";
    [SerializeField] private Vector3 spawnPoint = new Vector3(0,5,0);
    [SerializeField] private GameObject enemy;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
    }
    ////enemy spawn test
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.I))
    //    {
    //        var _enemy = PhotonNetwork.Instantiate(enemy.name, spawnPoint.position, Quaternion.identity);
    //    }
    //}
    public void Connect(GameObject selectedChar, string roomName)
    {
        player = selectedChar;
        this.roomName = roomName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SceneManager.LoadScene("NetworkTest");
        PhotonNetwork.JoinOrCreateRoom(roomName, null,null);       
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer(true);
    }
}
