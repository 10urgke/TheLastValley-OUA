using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject charSelectPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private Vector2 mainPanelMoveLoc;
    [SerializeField] private float mainPanelTime;
    [SerializeField] private float charSelectPanelTime;

    [Space]
    [SerializeField] RoomManager roomManager;
    public GameObject wariorPrefab, archerPrefab, wizardPrefab;
    public TMP_InputField roomNameInput;
    public ParticleSystem inputFx;

    public void ClickStartButton()
    {
        LeanTween.moveLocal(mainPanel, mainPanelMoveLoc, mainPanelTime).setEase(LeanTweenType.easeOutExpo)
            .setOnComplete(() =>
            {
                LeanTween.scale(charSelectPanel, Vector3.one, charSelectPanelTime).setEase(LeanTweenType.easeOutExpo);
            });     
    }

    public void ClickExitButton()
    {
    #if UNITY_STANDALONE
            Application.Quit();
    #endif

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    public void ConnectWithWarior()
    {
        if(roomNameInput.text == "")
        {
            ShowInputFieldFx();
            return;
        }    

        charSelectPanel.SetActive(false);
        connectPanel.SetActive(true);
        roomManager.Connect(wariorPrefab, roomNameInput.text);
    }

    public void ConnectWitArcher()
    {
        if (roomNameInput.text == "")
        {
            ShowInputFieldFx();
            return;
        }

        charSelectPanel.SetActive(false);
        connectPanel.SetActive(true);
        roomManager.Connect(archerPrefab, roomNameInput.text);
    }
    public void ConnectWitWizard()
    {
        if (roomNameInput.text == "")
        {
            ShowInputFieldFx();
            return;
        }

        charSelectPanel.SetActive(false);
        connectPanel.SetActive(true);
        roomManager.Connect(wizardPrefab, roomNameInput.text);
    }

    public void ShowInputFieldFx()
    {
        inputFx.Play();
    }
}
