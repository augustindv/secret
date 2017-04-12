using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMoney : MonoBehaviour {

    public Text money;
    public Text auctionMoney;

	public void ChangeMoneyInt(int newMoney)
    {
        money.text = newMoney.ToString();
        if (auctionMoney != null)
            auctionMoney.text = newMoney.ToString();
    }
}
