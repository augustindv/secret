using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionBankController : MonoBehaviour {
    private static DecisionBankController decisionBankController;
    public GameObject uiDecisionBank;
    public Text playerMoney;
    public Text extraMoney;
    public Button grabMoneyButton;

    public static DecisionBankController instance
    {
        get
        {
            if (!decisionBankController)
            {
                decisionBankController = FindObjectOfType(typeof(DecisionBankController)) as DecisionBankController;

                if (!decisionBankController)
                {
                    Debug.LogError("There needs to be one active DecisionBankController script on a GameObject in your scene.");
                }
            }

            return decisionBankController;
        }
    }

    private void Start()
    {
        grabMoneyButton.onClick.AddListener(delegate {
            GrabMoney();
            grabMoneyButton.interactable = false;
        });
    }

    public void StartPhase ()
    {
        // TODO anim in
        AnimationController.instance.PlayBankAnimation();
        uiDecisionBank.SetActive(true);
        grabMoneyButton.interactable = true;
        AudioController.instance.PlayLocalFx("DecisionBank");
    }

    public void GrabMoney()
    {
        int money = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney;
        int extra = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoneyExtra;

        // TODO anim out
        //playerMoney.text = "MONEY " + money;
        //extraMoney.text = extra + "";

        MoneyController.instance.MoneyHasChanged(money);

        // notify sequencer
        //PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).CmdIsReadyForNextPhase(true);
        StartCoroutine(AnimationController.instance.StopCurrentAnimationMainOutPhase(false, true));
        //uiDecisionBank.SetActive(false);
    }

    public void StopPhase()
    {
        // TODO anim out
        AnimationController.instance.StopWaitingAnimation();
    }

    public void SetExtraMoneyText(int money)
    {
        extraMoney.text = money + "";
    }

    // TODO use sync var hook + event
    void Update()
    {
        /*if(PlayerDatabase.instance.PlayerName != null)
        {
            if(playerMoney.text != "MONEY " + PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney)
            {
                playerMoney.text = "MONEY " + PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney;
                extraMoney.text = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoneyExtra + "";
            }
        }*/

    }
}
