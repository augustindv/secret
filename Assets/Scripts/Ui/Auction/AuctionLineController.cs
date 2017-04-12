using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ChangeMoneyBidEvent : UnityEvent<int>
{
}

[System.Serializable]
public class ChangeProgressBarEvent : UnityEvent<float>
{
}

public class AuctionLineController : MonoBehaviour {

    public static int MAX_PROGRESS_BAR_WIDTH = 700;

    public int moneyBid = 0;
    public int index;

    public static AuctionLineController auctionLineController;
    public static AuctionLineController instance
    {
        get
        {
            if (!auctionLineController)
            {
                auctionLineController = FindObjectOfType(typeof(AuctionLineController)) as AuctionLineController;

                if (!auctionLineController)
                {
                    Debug.LogError("There needs to be one active AuctionLineController script on a GameObject in your scene.");
                }
            }

            return auctionLineController;
        }
    }

    public ChangeMoneyBidEvent onChangeMoneyBid;
    public ChangeProgressBarEvent onChangeProgressBar;

    public delegate void ChangeMoneyBid(int newMoney);
    public event ChangeMoneyBid MoneyChanged;
    public delegate void ChangeProgressBar(float newWidth);
    public event ChangeProgressBar ProgressBarChanged;

    public void MoneyBidHasChanged(int newMoney, float newWidth)
    {
        if (MoneyChanged != null)
            MoneyBidHasChanged(newMoney, newWidth);
        if (ProgressBarChanged != null)
            MoneyBidHasChanged(newMoney, newWidth);

        onChangeMoneyBid.Invoke(newMoney);
        onChangeProgressBar.Invoke(newWidth);
    }

    public Button buttonToBid;

    void Start()
    {
        MoneyController.instance.MoneyHasChanged(UiMainController.instance.localPlayer.playerMoney);
        buttonToBid.onClick.AddListener(ClickToBid);
        MoneyBidHasChanged(moneyBid, 0);
    }

    void ClickToBid()
    {
        if (UiMainController.instance.localPlayer.playerMoney != 0)
        {
            UiMainController.instance.localPlayer.CmdUpdateBidOnAuction(index);
            UiMainController.instance.localPlayer.playerMoney--;
            MoneyController.instance.MoneyHasChanged(UiMainController.instance.localPlayer.playerMoney);
        }
    }

}
