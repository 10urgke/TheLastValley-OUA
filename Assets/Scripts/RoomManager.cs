using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player;
    [SerializeField] private string roomName = "test";
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private GameObject enemy;
    void Start()
    {
        spawnPoint = new Vector3(445 + Random.Range(-5, +5), 5, 455 + Random.Range(-5, +5));
        DontDestroyOnLoad(gameObject);
    }

    //enemy spawn test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            var _enemy = PhotonNetwork.Instantiate(enemy.name, spawnPoint, Quaternion.identity);
        }
    }

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
        SceneManager.LoadScene("Terra");
        PhotonNetwork.JoinOrCreateRoom(roomName, null,null);       
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer(true);
    }
}
