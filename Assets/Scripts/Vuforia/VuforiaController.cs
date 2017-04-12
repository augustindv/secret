using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VuforiaController : MonoBehaviour {

    public static VuforiaController vuforiaController;

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

    public void Start()
    {
        scanCard.onClick.AddListener(GoToScanCard);
        back.onClick.AddListener(GoBackToinventory);
    }

    public void GoToScanCard()
    {
        vuforiaObject.SetActive(true);
        UiMainController.instance.uiMain.SetActive(false);
    }

    public void GoBackToinventory()
    {
        UiMainController.instance.localPlayer.DiscoverSecret(associatedCard.deckID, associatedCard.cardID);
        vuforiaObject.SetActive(false);
        UiMainController.instance.uiMain.SetActive(true);
    }

    public void SaveSecret(Card associatedCard)
    {
        VuforiaController.instance.associatedCard = associatedCard;
    }
}
