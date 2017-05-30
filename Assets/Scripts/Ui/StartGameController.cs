using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StartGameController : MonoBehaviour {

    public static int LIMIT_NAME = 10;

    public GameObject selfiePanel;
    public GameObject namePanel;
    public GameObject readyPanel;
    public GameObject connectionPanel;
    public GameObject searchPanel;
    public GameObject creditsPanel;

    public InputField nameField;
    public Button okNameButton;
    public Button okSelfieButton;
    public Button readyButton;
    public Button creditsButton;
    public Button backButton;
    //public Button autoconnectButton;
    public Button joinButton;
    public Button hostButton;
    public Button cancelButton;
    public Text playersConnected;
    public Text playersReady;

    private CaptainsMess mess;
    public bool gameStarted;

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
        //nameField.onEndEdit.AddListener(delegate { if(nameField.text.Length >= 2) OKName(); });
        okSelfieButton.onClick.AddListener(OKSelfie);
        //autoconnectButton.onClick.AddListener(Autoconnect);
        hostButton.onClick.AddListener(Host);
        joinButton.onClick.AddListener(Join);
        cancelButton.onClick.AddListener(Cancel);
        creditsButton.onClick.AddListener(Credits);
        backButton.onClick.AddListener(Back);
        nameField.onValueChanged.AddListener(delegate { okNameButton.interactable = nameField.text.Length >= 2; });
        mess = FindObjectOfType(typeof(CaptainsMess)) as CaptainsMess;
        gameStarted = false;

        connectionPanel.SetActive(true);
    }

    public void OpenWebsite(int siteNb)
    {
        switch (siteNb)
        {
            case 0:
                Application.OpenURL("http://yasminaazeri.fr/");
                break;
            case 1:
                Application.OpenURL("http://www.augustindevita.com");
                break;
            case 2:
                Application.OpenURL("http://www.nicolasdufort.com");
                break;
            case 3:
                Application.OpenURL("http://www.corradolongoni.xyz");
                break;
            case 4:
                Application.OpenURL("http://www.maximeneveu.net");
                break;
            case 5:
                Application.OpenURL("http://www.manontardif.fr");
                break;
            default:
                break;
        }
    }

    public void Run()
    {
        AnimationController.instance.StopWaitingAnimation();
        UiMainController.instance.uiStartGame.SetActive(true);
        namePanel.SetActive(false);
        selfiePanel.SetActive(false);
        searchPanel.SetActive(false);
        readyPanel.SetActive(true);
    }

    public void StartPersonnalisation()
    {
        gameStarted = true;
        if (skip)
        {
            UiMainController.instance.localPlayer.SetPlayerName(
                skipNames[int.Parse(UiMainController.instance.localPlayer.netId.ToString())]);
            StartCoroutine(AutoSelfie());
        }
        else
        {
            cancelButton.gameObject.SetActive(false);
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

    public void OKName()
    {
        AudioController.instance.PlayLocalFx("IntroOk");
        Player localPlayer = UiMainController.instance.localPlayer;
        localPlayer.SetPlayerName(nameField.text);
        selfiePanel.SetActive(true);
        okNameButton.interactable = false;
        namePanel.SetActive(false);
    }

    public void OKSelfie()
    {
        AudioController.instance.PlayLocalFx("IntroPhoto");
        Player localPlayer = UiMainController.instance.localPlayer;
        okNameButton.interactable = false;
        localPlayer.SnapSelfie();
    }

    public void Autoconnect()
    {
        mess.AutoConnect();
        connectionPanel.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        searchPanel.SetActive(true);
        // TODO : panel waiting for connection
        AnimationController.instance.PlayWaitingAnimation();
    }

    public void Host()
    {
        mess.StartHosting();
        connectionPanel.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        searchPanel.SetActive(true);
        // TODO : panel waiting for connection
        AnimationController.instance.PlayWaitingAnimation();
    }

    public void Join()
    {
        mess.StartJoining();
        connectionPanel.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        searchPanel.SetActive(true);
        // TODO : panel waiting for connection
        AnimationController.instance.PlayWaitingAnimation();
    }

    public void Cancel()
    {
        mess.Cancel();
        connectionPanel.SetActive(true);
        readyPanel.SetActive(false);
        searchPanel.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        AnimationController.instance.StopWaitingAnimation();
    }

    public void Credits()
    {
        connectionPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void Back()
    {
        connectionPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (int.Parse(playersConnected.text) != mess.Players().Count)
            {
                playersConnected.text = mess.Players().Count + "";
            }

            List<CaptainsMessPlayer> tmp = mess.Players().FindAll(p => p.IsReady());
            if (int.Parse(playersReady.text) != tmp.Count)
            {
                playersReady.text = tmp.Count + "";
            }
        }
    }

}

