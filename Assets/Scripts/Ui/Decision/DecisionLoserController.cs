using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionLoserController : MonoBehaviour {
    private static DecisionLoserController decisionLoserController;
    public GameObject uiDecisionLoser;
    public Button exitButton;
     
    public static DecisionLoserController instance
    {
        get
        {
            if (!decisionLoserController)
            {
                decisionLoserController = FindObjectOfType(typeof(DecisionLoserController)) as DecisionLoserController;

                if (!decisionLoserController)
                {
                    Debug.LogError("There needs to be one active DecisionBankController script on a GameObject in your scene.");
                }
            }

            return decisionLoserController;
        }
    }

    // Use this for initialization
    void Start () {
        exitButton.onClick.AddListener(delegate {
            //PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).CmdIsReadyForNextPhase(true);
            exitButton.interactable = false;
            uiDecisionLoser.SetActive(false);
            StartCoroutine(AnimationController.instance.StopCurrentAnimationMainOutPhase(false, true));
        });
    }
	
    public void StartPhase()
    {
        AnimationController.instance.PlayLoserAnimation();
        uiDecisionLoser.SetActive(true);
        exitButton.interactable = true;
        AudioController.instance.PlayLocalFx("GenericFail");
    }
}
