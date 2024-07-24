using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPun
{
    public List<GameObject> quests;
    public float nextQuestTimeDuration = 300f;
    public float nextQuestRemainingTime;
    public float questTimeDuration = 300f;
    public float questRemainingTime;
    private bool isQuestRunning = false;
    private bool isNextQuestTimerRunning = false;
    private GameObject currentQuest;
    public TextMeshProUGUI timerText;

    private void Update()
    {
        //test
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.T))
        {
            StartNextQuestTimer();
        }

        if (isNextQuestTimerRunning)
        {
            nextQuestRemainingTime -= Time.deltaTime;
            if (nextQuestRemainingTime <= 0)
            {
                nextQuestRemainingTime = 0;
                isNextQuestTimerRunning = false;
                StartQuest();
            }
            UpdateNextQuestTimerUI();
        }


        if (isQuestRunning)
        {
            questRemainingTime -= Time.deltaTime;
            if (questRemainingTime <= 0)
            {
                questRemainingTime = 0;
                isQuestRunning = false;
                photonView.RPC("EndGame", RpcTarget.All);
            }
            UpdateQuestTimerUI();
        }
    }
    public void SetTimerText(TextMeshProUGUI TMPtext)
    {
        timerText = TMPtext;
    }
    private void UpdateNextQuestTimerUI()
    {
        int minutes = Mathf.FloorToInt(nextQuestRemainingTime / 60F);
        int seconds = Mathf.FloorToInt(nextQuestRemainingTime % 60F);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    private void UpdateQuestTimerUI()
    {
        int minutes = Mathf.FloorToInt(questRemainingTime / 60F);
        int seconds = Mathf.FloorToInt(questRemainingTime % 60F);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void StartNextQuestTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            nextQuestRemainingTime = nextQuestTimeDuration;
            isNextQuestTimerRunning = true;
            timerText.color = Color.yellow;
            photonView.RPC("SyncNextQuestTimer", RpcTarget.All, nextQuestRemainingTime, isNextQuestTimerRunning);

        }
    }
    public void StartQuest()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            questRemainingTime = questTimeDuration;
            isQuestRunning = true;
            int randomQuestIndex = Random.Range(0, quests.Count);
            currentQuest = quests[randomQuestIndex];
            currentQuest.SetActive(true);
            timerText.color = Color.red;
            photonView.RPC("SyncQuestTimer", RpcTarget.All, questRemainingTime, isQuestRunning, randomQuestIndex);
        }
    }
    public void CompleteQuest()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isQuestRunning = false;
            currentQuest.SetActive(false);
            StartNextQuestTimer();
        }
    }
    [PunRPC]
    private void SyncNextQuestTimer(float time, bool running)
    {
        nextQuestRemainingTime = time;
        isNextQuestTimerRunning = running;
        if (isNextQuestTimerRunning)
        {
            timerText.color = Color.yellow;
            UpdateNextQuestTimerUI();
        }
    }
    [PunRPC]
    private void SyncQuestTimer(float time, bool running, int questIndex)
    {
        questRemainingTime = time;
        isQuestRunning = running;
        currentQuest = quests[questIndex];
        currentQuest.SetActive(true);
        if (isQuestRunning)
        {
            timerText.color = Color.red;
            UpdateQuestTimerUI();
        }
    }

    [PunRPC]
    private void EndGame()
    {
        Debug.Log("End game");
    }

    public void CheckAllPlayersDead()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                ThirdPersonCharacterController controller = player.GetComponent<ThirdPersonCharacterController>();
                if (!controller.isDead)
                {
                    Debug.Log(controller.gameObject + " is not dead");
                    return;
                }
            }
            photonView.RPC("AllPlayersDead", RpcTarget.All);
        }
    }

    [PunRPC]
    private void AllPlayersDead()
    {
        // load end scene or different one
        photonView.RPC("EndGame", RpcTarget.All);
    }
}
