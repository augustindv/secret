using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionBankController : MonoBehaviour {
    private static DecisionBankController decisionBankController;
    public GameObject uiDecisionBank;
    public Text playerMoney;
    public Text extraMoney;

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
        uiDecisionBank.transform.FindChild("GrabMoney").GetComponent<Button>().onClick.AddListener(delegate {
            GrabMoney();
        });
    }

    public void StartPhase ()
    {
        // TODO anim in
        uiDecisionBank.SetActive(true);
    }

    public void GrabMoney()
    {
        int money = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney;
        int extra = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoneyExtra;

        // TODO anim out
        playerMoney.text = "MONEY " + money;
        extraMoney.text = "EXTRA MONEY " + extra;

        MoneyController.instance.MoneyHasChanged(money);

        // notify sequencer
        PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).CmdIsReadyForNextPhase(true);
    }

    // Update is called once per frame
    public void StopPhase()
    {   
        // TODO anim out
        uiDecisionBank.SetActive(false);
    }

    // TODO use sync var hook + event
    void Update()
    {
        if(PlayerDatabase.instance.PlayerName != null)
        {
            if(playerMoney.text != "MONEY " + PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney)
            {
                playerMoney.text = "MONEY " + PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoney;
                extraMoney.text = "EXTRA MONEY " + PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).playerMoneyExtra;
            }
        }

    }
}
