using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class VuforiaController : MonoBehaviour {

    public static VuforiaController vuforiaController;
    public delegate void VuforiaScanDone(int deckId, int cardId);
    public bool skip = false;

    public static VuforiaController instance
    {
        get
        {
            if (!vuforiaController)
            {
                vuforiaController = FindObjectOfType(typeof(VuforiaController)) as VuforiaController;

                if (!vuforiaController)
                {
                    Debug.LogError("There needs to be one active VuforiaController script on a GameObject in your scene.");
                }

            }

            return vuforiaController;
        }
    }

    public Button scanCard;
    public Button finishScanButton;
    public Button finishScanButtonDecision;
    public GameObject vuforiaObject;
    [HideInInspector]
    public Card associatedCard;
    private VuforiaScanDone vuforiaScanDone;
    public GameObject deckScanPanel;
    public GameObject cardScanPanel;
    public Player playerToScan;

    public Text debug;
    public Button OKScanTuto;

    public void Start()
    {
        scanCard.onClick.AddListener(GoToScanCard);
        finishScanButton.onClick.AddListener(FinishScan);
        finishScanButton.gameObject.SetActive(false);
        finishScanButtonDecision.onClick.AddListener(FinishScan);
        finishScanButtonDecision.gameObject.SetActive(false);
    }

    public void Reset()
    {
        finishScanButton.gameObject.SetActive(false);
        finishScanButtonDecision.gameObject.SetActive(false);
        CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    public void GoToScanCard()
    {
        Reset();
        vuforiaObject.SetActive(true);
        UiMainController.instance.uiMain.SetActive(false);
    }

    public void ScanCard(string targetName, VuforiaScanDone vuforiaScanDone)
    {
        finishScanButton.gameObject.SetActive(false);
        finishScanButtonDecision.gameObject.SetActive(false);
        playerToScan = PlayerDatabase.instance.GetPlayer(targetName);
        vuforiaObject.SetActive(true);
        deckScanPanel.SetActive(false);
        cardScanPanel.SetActive(true);
        cardScanPanel.transform.FindChild("Notice").GetComponent<Text>().text = "Scannez la carte de <color=" + PlayerDatabase.instance.GetColorHex(targetName) +">" + targetName + "</color>";
        this.vuforiaScanDone = vuforiaScanDone;
    }

    public void FinishScan()
    {
        if(associatedCard != null && this.vuforiaScanDone != null && associatedCard.deckID == PlayerDatabase.instance.GetPlayerDeckID(playerToScan))
        {
            this.vuforiaScanDone(associatedCard.deckID, associatedCard.cardID);
            this.vuforiaScanDone = null;
            vuforiaObject.SetActive(false);
            finishScanButton.gameObject.SetActive(false);
            finishScanButtonDecision.gameObject.SetActive(false);
            AnimationController.instance.PlayWaitingAnimation();
            return;
        }

        if (associatedCard != null && UiMainController.instance.localPlayer.gamePhase == GamePhase.ScanDeck && associatedCard.cardID == -1)
        {
            UiMainController.instance.localPlayer.CmdSetPlayerCardDeck(associatedCard.deckID);
            finishScanButton.gameObject.SetActive(false);
            finishScanButtonDecision.gameObject.SetActive(false);
            vuforiaObject.SetActive(false);
            AnimationController.instance.PlayWaitingAnimation();
        } else if (associatedCard != null && associatedCard.deckID == PlayerDatabase.instance.GetPlayerDeckID(playerToScan))
        {
            UiMainController.instance.localPlayer.DiscoverSecret(associatedCard.deckID, associatedCard.cardID);
            vuforiaObject.SetActive(false);
            UiMainController.instance.uiMain.SetActive(true);
        } 
    }

    public void SaveSecret(Card associatedCard)
    {
        if ((UiMainController.instance.localPlayer.gamePhase == GamePhase.ScanDeck && associatedCard.cardID == -1) || (associatedCard.cardID != -1 && associatedCard.deckID == PlayerDatabase.instance.GetPlayerDeckID(playerToScan)))
        {
            VuforiaController.instance.associatedCard = associatedCard;
            finishScanButton.gameObject.SetActive(true);
            finishScanButtonDecision.gameObject.SetActive(true);
            AudioController.instance.PlayLocalFx("ScanOk");
        }
    }
}
