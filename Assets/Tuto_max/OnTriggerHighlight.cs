using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerHighlight : MonoBehaviour {

	public GameObject PlayerTrigger;
	public GameObject Highlight;
	public GameObject BankScreen;
	public Button BankScreenButton;
	int count;

	public GameObject DecisionCibleBank;
	public GameObject DecisionCibleJoueur;

    public GameObject decisionBankTarget;

    public GameObject PlayerMarker;

	// Use this for initialization
	void Start () {
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D PlayerTrigger) {
		Highlight.SetActive (true);
        decisionBankTarget.GetComponent<OnTriggerHighlightAvatar>().active = true;
        DecisionCibleBank.SetActive (false);
		if (count == 0) {
			StartCoroutine (Example ());
		}
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(.5f);
		BankScreen.SetActive (true);
		count++;
	}

	void OnTriggerExit2D(Collider2D PlayerTrigger) {
		Highlight.SetActive (false);
		Debug.Log ("no trigger");
		if (count == 1) {
			PlayerMarker.SetActive (false);
			BankScreenButton.onClick.AddListener(BankScreenOff);
		}
			
	}

	void BankScreenOff() {
		BankScreen.SetActive (false);
		PlayerMarker.SetActive (true);	
		DecisionCibleJoueur.SetActive (true);
		count++;
	}


}
