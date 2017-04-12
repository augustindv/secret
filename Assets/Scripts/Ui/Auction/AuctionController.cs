using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AuctionController : MonoBehaviour {

    public static AuctionController auctionController;
    public static AuctionController instance
    {
        get
        {
            if (!auctionController)
            {
                auctionController = FindObjectOfType(typeof(AuctionController)) as AuctionController;

                if (!auctionController)
                {
                    Debug.LogError("There needs to be one active AuctionController script on a GameObject in your scene.");
                }
            }

            return auctionController;
        }
    }

    public GameObject auctionLinePrefab;
    public List<GameObject> auctionLines = new List<GameObject>();

    public void AddAuctionLine(CheckID checkIDToPublish)
    {
        GameObject auctionLine = Instantiate(auctionLinePrefab, GameObject.Find("UiAuction").transform.FindChild("Auction"));
        auctionLine.GetComponent<AuctionLineController>().index = auctionLines.Count;
        Sprite sprite = PlayerDatabase.instance.GetSprite(PlayerDatabase.instance.GetName(checkIDToPublish.playerOwner));
        auctionLine.transform.FindChild("Progress").FindChild("Head").GetComponent<Image>().sprite = sprite;
        auctionLines.Add(auctionLine);
    }

    public void UpdateBidOnAuction(int index)
    {
        AuctionLineController auction = auctionLines[index].GetComponent<AuctionLineController>();
        auction.moneyBid++;
        auction.MoneyBidHasChanged(auction.moneyBid, auction.moneyBid * AuctionLineController.MAX_PROGRESS_BAR_WIDTH / Bank.instance.totalMoneyInGame);
    }

    public CheckID CheckToRevealed()
    {
        int index = 0;
        int moneyBidRef = 0;
        List<CheckID> winners = new List<CheckID>();
        foreach (GameObject auctionLine in auctionLines)
        {
            int tmpMoneyBid = auctionLine.GetComponent<AuctionLineController>().moneyBid;
            if (tmpMoneyBid >= moneyBidRef)
            {
                winners.Add(AuctionData.instance.checksIDToPublish[index]);
                moneyBidRef = tmpMoneyBid;
            }
            index++;
        }

        winners.Shuffle();
        return winners.Count != 0 ? winners.First() : new CheckID();
    }

}
