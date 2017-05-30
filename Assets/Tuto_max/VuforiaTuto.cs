using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VuforiaTuto : MonoBehaviour {

    public Button OKScan;
	public GameObject UiVuforia;
	public GameObject TutoVuforiaButton;
	public GameObject UiMain;
	public GameObject MainBlackBackground;
	public GameObject uiMainTuto01;
	public GameObject uiMainTuto02;
	public GameObject uiMainTuto03;

	public GameObject SecretPopUp;
	public GameObject SecretCloseScreen;
	public Button SecretCloseButton;

    public Button GoToPublishingButton;

	public GameObject AuctionTuto;
	public Button AuctionClose;
	public GameObject GoToAuction;
	public Button GoToAuctionButton;
	public GameObject TutoAuctionGo;
	public GameObject TutoAuctionScreen;

	public GameObject AuctionScreen;
	public GameObject TutoAuctionUI;

    public GameObject inventoryContent;
    public GameObject secretTuto;

    public GameObject uiPublishing;
    public GameObject publishingContent;
    public GameObject secretPublishingTuto;
    public GameObject uiPublishingTuto;
    public Button uiPublishingButton;
    public GameObject uiPublishingNon;
    public Button uiOkPublishingBt;
    public GameObject PublishingPopUp;
    public GameObject PublishButton;
    public GameObject PublishButtonNo;

    // Use this for initialization
    void Start () {
        OKScan.onClick.AddListener(GoToSecret);
    }

	void GoToSecret() {
		UiMain.SetActive (true);
		MainBlackBackground.SetActive (true);
		UiVuforia.SetActive (false);
		uiMainTuto01.SetActive (false);
		uiMainTuto02.SetActive (false);
		uiMainTuto03.SetActive (false);
		SecretPopUp.SetActive (true);
		SecretCloseScreen.SetActive (true);
		SecretCloseButton.onClick.AddListener (CloseSecret);
		TutoVuforiaButton.SetActive(false);
        Transform secretFoundTr = inventoryContent.transform.FindChild("SecretsFound");
        secretFoundTr.transform.localScale = new Vector3(1, 1, 1);
        Destroy(secretFoundTr.FindChild("InventorySecretsEmpty").gameObject);
        Instantiate(secretTuto, secretFoundTr);
    }

	void CloseSecret() {
		SecretPopUp.SetActive (false);
		SecretCloseScreen.SetActive (false);
		MainBlackBackground.SetActive (false);
		AuctionTuto.SetActive (true);
		AuctionClose.onClick.AddListener (CloseExplain);

	}

	void CloseExplain() {
		GoToAuction.SetActive (true);
		TutoAuctionGo.SetActive (true);
		GoToPublishingButton.onClick.AddListener (GoPublishing);
		AuctionTuto.SetActive (false);
	}

    void GoPublishing()
    {
        UiMain.SetActive(false);
        TutoAuctionGo.SetActive(false);
        GoToAuction.SetActive(false);
        uiPublishing.SetActive(true);
        uiPublishingTuto.SetActive(true);
        uiOkPublishingBt.onClick.AddListener(OpenPopUp);
        uiPublishingButton.onClick.AddListener(Non);
        Transform secretFoundTr = publishingContent.transform;
        Instantiate(secretTuto, secretFoundTr);
        GoToAuctionButton.onClick.AddListener(GOAuction);
    }

    void OpenPopUp()
    {
        PublishingPopUp.SetActive(true);
        PublishButton.SetActive(true);
        PublishButtonNo.SetActive(false);
        uiPublishingNon.SetActive(false);
        uiPublishingButton.onClick.AddListener(GOAuction);
    }

    void Non()
    {
        uiPublishingNon.SetActive(true);
        
    }

	void GOAuction() {
        uiPublishing.SetActive (false);
        uiPublishingTuto.SetActive (false);
		AuctionScreen.SetActive (true);
		TutoAuctionScreen.SetActive (true);
		TutoAuctionUI.SetActive (true);
  
	}
}
