using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnQuestManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<GameObject> enemies;
    public List<Transform> destinations;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnEnable()
    {
        QuestStart();
    }

    public void QuestStart()
    {
        foreach (GameObject enemy in enemies)
            enemy.SetActive(true);
    }

    [PunRPC]
    public void CompleteQuest()
    {
        gameManager.CompleteQuest();
        gameObject.SetActive(false);
    }
}
