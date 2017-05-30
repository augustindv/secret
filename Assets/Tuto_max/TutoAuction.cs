using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoAuction : MonoBehaviour {

	public GameObject TutoAuctionUI;
	public GameObject Auction01;
	public GameObject Auction02;

	public GameObject TutoAuctionScreen;
	public GameObject UiAuction;
	public GameObject Bid;
	public Button BidButton;

	public GameObject TutoPublication;
	public GameObject UiRevelation;
	public GameObject PublicationScreen;
	public Button PublicationButton;
	public GameObject PublicationWait;
	public Button PublicationWaitButton;
	public GameObject EndScreen;
	public Button EndButton;

	float Wbid;

	// Use this for initialization
	void Start () {
		BidButton.onClick.AddListener (bidding);
		RectTransform rtBid = (RectTransform)Bid.transform;
		Wbid = rtBid.rect.width;
		PublicationScreen.SetActive (false);
		Auction01.SetActive (true);
	}

	void bidding() {
		Wbid+= 50 ;
		Bid.GetComponent<RectTransform>().sizeDelta = new Vector2(Wbid, Bid.GetComponent<RectTransform>().sizeDelta.y);
		ChangeMoney ();
		Debug.Log (Wbid);
		if (Wbid >= 50)
			Auction01.SetActive (false); Auction02.SetActive (true);
	}

	public Text money;
	public Text auctionMoney;
	int newMoney = 10;
	int newBid;

	public void ChangeMoney()
	{
		newMoney--;
		money.text = newMoney.ToString ();
		newBid++;
		auctionMoney.text = newBid.ToString ();
		Debug.Log (newMoney);
		if (newMoney == 5) {
			GoToPublication ();
		}
	}

	void GoToPublication() {
		TutoAuctionUI.SetActive (false);
		TutoPublication.SetActive (true);
		PublicationScreen.SetActive (true);
		UiRevelation.SetActive (true);
		PublicationButton.onClick.AddListener (Revelation);
	}

	void Revelation() {
		PublicationScreen.SetActive (false);
		PublicationWait.SetActive (true);
		PublicationWaitButton.onClick.AddListener (End);
        AnimationController.instance.PlayAnimationSecretTuto("Maxime", 22);
    }

	void End() {
		UiRevelation.SetActive (false);
		PublicationWait.SetActive (false);
		EndScreen.SetActive (true);
		EndButton.onClick.AddListener(Restart);
        AnimationController.instance.StopAnimationSecret();
    }

    void Restart() {
        UiMainController.instance.inTuto = false;
        SceneManager.LoadScene(0); 
	}

}
