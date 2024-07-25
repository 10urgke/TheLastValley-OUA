using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerQuestManager : MonoBehaviourPun
{
    public GameObject villager;
    public GameManager gameManager;
    public Vector3 villagerStartPosition;
    public Transform destination;
    public List<GameObject> firstEnemies;
    public List<GameObject> secondEnemies;
    public List<Vector3> firstEnemiesPos;
    public List<Vector3> secondEnemiesPos;
    public bool questRunning;
    private void Start()
    {
        villagerStartPosition = villager.transform.localPosition;
        gameManager = FindAnyObjectByType<GameManager>();
        questRunning = false;

    }

    public void QuestStart()
    {
        if (!questRunning)
        {
            foreach (GameObject enemy in firstEnemies)
                enemy.SetActive(true);
        }
        
        questRunning = true;
    }

    public void SecondPhaseInQuest()
    {
        foreach (GameObject enemy in secondEnemies)
            enemy.SetActive(true);
    }
    [PunRPC]
    public void CompleteQuest()
    {
        for (int i = 0; i < firstEnemies.Count; i++)
        {
            firstEnemies[i].transform.localPosition = firstEnemiesPos[i];
            firstEnemies[i].SetActive(false);
        }
        for (int i = 0; i < secondEnemies.Count; i++)
        {
            secondEnemies[i].transform.localPosition = secondEnemiesPos[i];
            secondEnemies[i].SetActive(false);
        }
        villager.transform.localPosition = villagerStartPosition;
        questRunning = false;
        gameManager.CompleteQuest();
        gameObject.SetActive(false);
        
    }
}
