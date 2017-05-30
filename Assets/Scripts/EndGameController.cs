using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour {

    public Button goBackToDiscussion;

	void Start () {
        goBackToDiscussion.onClick.AddListener(delegate
        {
            AnimationController.instance.StopEndGameAnimation();
            UiMainController.instance.uiEndGame.SetActive(false);
        });
    }

}
