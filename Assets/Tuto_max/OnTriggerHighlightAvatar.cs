using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerHighlightAvatar : MonoBehaviour {

	public GameObject PlayerTrigger;
	public GameObject Highlight;
	public GameObject PlayerScreen;
	public Button PlayerScreenButton;
	public GameObject PlayerMarker;
	int count;
    public bool active;

	public GameObject DecisionCibleJoueur;

	// Use this for initialization
	void Start () {
		count = 0;
	}

	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D PlayerTrigger) {
        if (active)
        {
            Highlight.SetActive(true);
            DecisionCibleJoueur.SetActive(false);
            if (count == 0)
            {
                StartCoroutine(Example());
                count++;
            }
        }
	}

		IEnumerator Example() {
			yield return new WaitForSeconds(.5f);
			PlayerScreen.SetActive (true);
		}

	void OnTriggerExit2D(Collider2D PlayerTrigger) {
		Highlight.SetActive (false);
		if (count == 1) {
			PlayerMarker.SetActive (false);
			PlayerScreenButton.onClick.AddListener(PlayerScreenOff);
		}
	}

	void PlayerScreenOff() {
		PlayerScreen.SetActive (false);
		count++;
	}

}
