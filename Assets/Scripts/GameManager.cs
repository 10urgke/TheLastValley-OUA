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
    public bool isQuestRunning = false;
    public bool isNextQuestTimerRunning = false;
    private GameObject currentQuest;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI questCountText;
    public int questCount = 0;
    public GameObject finalBoss;

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
                if(photonView.IsMine)
                    photonView.RPC("EndGame", RpcTarget.All);
            }
            UpdateQuestTimerUI();
        }
    }
     
    public void SetTimerText(TextMeshProUGUI TMPtext)
    {
        timerText = TMPtext;
    }

    public void SetQuestCountText(TextMeshProUGUI TMPtext)
    {
        questCountText = TMPtext;
        questCountText.color = Color.cyan;
    }

    private void UpdateNextQuestTimerUI()
    {
        int minutes = Mathf.FloorToInt(nextQuestRemainingTime / 60F);
        int seconds = Mathf.FloorToInt(nextQuestRemainingTime % 60F);
        timerText.text = string.Format("Next Quest: {0:0}:{1:00}", minutes, seconds);
    }

    private void UpdateQuestTimerUI()
    {
        int minutes = Mathf.FloorToInt(questRemainingTime / 60F);
        int seconds = Mathf.FloorToInt(questRemainingTime % 60F);
        timerText.text = string.Format("End Of World: {0:0}:{1:00}", minutes, seconds);
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
            questCount++;
            photonView.RPC("UpdateQuestCount", RpcTarget.All, questCount);

            if (questCount < 3)
                StartNextQuestTimer();
        }
    }

    [PunRPC]
    private void SyncNextQuestTimer(float time, bool running)
    {
        Debug.Log($"SyncNextQuestTimer called with time: {time} and running: {running}");
        nextQuestRemainingTime = time;
        isNextQuestTimerRunning = running;
        isQuestRunning = !running;
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
        isNextQuestTimerRunning = !running;
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
        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.LoadLevel("Fail");
    }
    [PunRPC]
    private void EndGameTwo()
    {
        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.LoadLevel("FailDead");
    }

    [PunRPC]
    private void UpdateQuestCount(int count)
    {
        questCount = count;
        questCountText.text = "Completed Quests: " + questCount;

        if (questCount >= 3)
        {
            isQuestRunning = false;
            isNextQuestTimerRunning = false;
            timerText.text = "The End is Near";
            timerText.color = Color.cyan;
            questCountText.color = Color.red;
            questCountText.text = "Destroy Boss!";
            finalBoss.SetActive(true);
        }
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
        photonView.RPC("EndGameTwo", RpcTarget.All);
    }
}
