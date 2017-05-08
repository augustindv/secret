﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button back;
    public GameObject vuforiaObject;
    [HideInInspector]
    public Card associatedCard;
    private VuforiaScanDone vuforiaScanDone;
    public GameObject deckScanPanel;
    public GameObject cardScanPanel;

    public Text debug;

    public void Start()
    {
        scanCard.onClick.AddListener(GoToScanCard);
        back.onClick.AddListener(FinishScan);
    }

    public void Reset()
    {
        back.gameObject.SetActive(true);
    }

    public void GoToScanCard()
    {
        Reset();
        vuforiaObject.SetActive(true);
        UiMainController.instance.uiMain.SetActive(false);
    }

    public void ScanCard(string targetName, VuforiaScanDone vuforiaScanDone)
    {
        vuforiaObject.SetActive(true);
        deckScanPanel.SetActive(false);
        cardScanPanel.SetActive(true);
        cardScanPanel.transform.FindChild("Notice").GetComponent<Text>().text = "Scannez la carte de " + targetName;
        this.vuforiaScanDone = vuforiaScanDone;
    }

    public void FinishScan()
    {
        if(this.vuforiaScanDone != null)
        {
            this.vuforiaScanDone(associatedCard.deckID, associatedCard.cardID);
            this.vuforiaScanDone = null;
            vuforiaObject.SetActive(false);
            return;
        }
        if (associatedCard.cardID != -1 && UiMainController.instance.localPlayer.gamePhase != GamePhase.ScanDeck)
        {
            UiMainController.instance.localPlayer.DiscoverSecret(associatedCard.deckID, associatedCard.cardID);
            vuforiaObject.SetActive(false);
            UiMainController.instance.uiMain.SetActive(true);
        } else if (UiMainController.instance.localPlayer.gamePhase == GamePhase.ScanDeck)
        {
            UiMainController.instance.localPlayer.CmdSetPlayerCardDeck(associatedCard.deckID);
            back.gameObject.SetActive(false);
        }

    }

    public void SaveSecret(Card associatedCard)
    {
        VuforiaController.instance.associatedCard = associatedCard;
    }
}
