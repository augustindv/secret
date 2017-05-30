using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoToScan : MonoBehaviour {

	public Button Bank;
	public Button Player;
	public GameObject GoToScanScreen;
	public Button GoToScanButton;
	bool BankCount;
	bool PlayerCount;
	public GameObject TutoDecision;

	public GameObject VuforiaScreen;


	public GameObject TutoVuforia;
	public GameObject TutoVuforiaUI;

	// Use this for initialization
	void Start () {
		BankCount = false;
		PlayerCount = false;
	}
	
	// Update is called once per frame
	void Update () {
		Bank.onClick.AddListener (OKBank);
		Player.onClick.AddListener (OKPlayer);

		if (BankCount == true && PlayerCount == true) {
			StartCoroutine (Example ());
		}
	}

		IEnumerator Example() {
			yield return new WaitForSeconds(1);
			GoToScanScreen.SetActive (true);
			GoToScanButton.onClick.AddListener (OKGoToScan);
		}

	void OKBank() {
		BankCount = true;
	}

	void OKPlayer() {
		PlayerCount = true;
	}

	void OKGoToScan() {
		BankCount = false;
		PlayerCount = false;
		GoToScanScreen.SetActive (false);
		TutoDecision.SetActive (false);
		VuforiaScreen.SetActive(true);
		TutoVuforia.SetActive (true);
		TutoVuforiaUI.SetActive(true);
	}
}
