using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour {

    public static int LIMIT_NAME = 15;

    public GameObject selfiePanel;
    public GameObject namePanel;
    public GameObject readyPanel;
    public GameObject connectionPanel;
    public InputField nameField;
    public Button okNameButton;
    public Button okSelfieButton;
    public Button readyButton;
    public Button autoconnectButton;

    private CaptainsMess mess;

    public GameObject selfie;

    public bool skip = false;
    private string[] skipNames = new string[] { "Da Bastard", "Herr Doktor Horror", "El Magnifico", "Machiavellistico", "Evil Genius"};

    private static StartGameController startGameController;

    public static StartGameController instance
    {
        get
        {
            if (!startGameController)
            {
                startGameController = FindObjectOfType(typeof(StartGameController)) as StartGameController;

                if (!startGameController)
                {
                    Debug.LogError("There needs to be one active StartGameController script on a GameObject in your scene.");
                }
            }

            return startGameController;
        }
    }

    public void Start()
    {
        okNameButton.onClick.AddListener(OKName);
        okSelfieButton.onClick.AddListener(OKSelfie);
        autoconnectButton.onClick.AddListener(Autoconnect);
        nameField.onValueChanged.AddListener(delegate { okNameButton.interactable = nameField.text.Length >= 2; });
        mess = FindObjectOfType(typeof(CaptainsMess)) as CaptainsMess;
    }

    public void Run()
    {
        UiMainController.instance.uiStartGame.SetActive(true);
        namePanel.SetActive(false);
        selfiePanel.SetActive(false);
        readyPanel.SetActive(true);
    }

    public void StartPersonnalisation()
    {
        if (skip)
        {
            UiMainController.instance.localPlayer.SetPlayerName(
                skipNames[int.Parse(UiMainController.instance.localPlayer.netId.ToString())]);
            StartCoroutine(AutoSelfie());
        }
        else
        {
            readyPanel.SetActive(false);
            namePanel.SetActive(true);
            okNameButton.interactable = false;
            nameField.characterLimit = LIMIT_NAME;
            selfiePanel.SetActive(false);
        }
    }

    IEnumerator AutoSelfie()
    {
        yield return new WaitForSeconds(1);
        UiMainController.instance.localPlayer.SnapSelfie();
    }

    public void Ready()
    {
        Player localPlayer = UiMainController.instance.localPlayer;
        if (localPlayer.IsReady())
        {
            localPlayer.SendNotReadyToBeginMessage();
        }
        else
        {
            localPlayer.SendReadyToBeginMessage();
        }
    }

    public void OKName()
    {
        Player localPlayer = UiMainController.instance.localPlayer;
        localPlayer.SetPlayerName(nameField.text);
        selfiePanel.SetActive(true);
        namePanel.SetActive(false);
    }

    public void OKSelfie()
    {
        Player localPlayer = UiMainController.instance.localPlayer;
        localPlayer.SnapSelfie();
    }

    public void Autoconnect()
    {
        mess.AutoConnect();
        connectionPanel.SetActive(false);
        // TODO : panel waiting for connection
        //namePanel.SetActive(true);
    }

}

