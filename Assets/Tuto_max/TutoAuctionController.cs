using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TutoAuctionController : MonoBehaviour {

	public static TutoAuctionController auctionController;
	public static TutoAuctionController instance
	{
		get
		{
			if (!auctionController)
			{
				auctionController = FindObjectOfType(typeof(TutoAuctionController)) as TutoAuctionController;

				if (!auctionController)
				{
					Debug.LogError("There needs to be one active AuctionController script on a GameObject in your scene.");
				}
			}

			return auctionController;
		}
	}

	public GameObject auctionLinePrefab;
	public GameObject auctionContent;
	public List<GameObject> auctionLines = new List<GameObject>();
	public int moneyBidTotal;

	public void ResetUiAuction()
	{
		auctionLines = new List<GameObject>();
		moneyBidTotal = 0;
		AuctionData.instance.CmdResetAuctionData();
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in auctionContent.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));
	}

	public void AddAuctionLine(CheckID checkIDToPublish, int index)
	{
		GameObject auctionLine = Instantiate(auctionLinePrefab, auctionContent.transform);
		auctionLine.GetComponent<AuctionLineController>().index = index;
		Sprite sprite = PlayerDatabase.instance.GetSprite(PlayerDatabase.instance.GetName(checkIDToPublish.playerOwner));
		auctionLine.transform.FindChild("Progress").FindChild("Chest").FindChild("Head").GetComponent<Image>().sprite = sprite;
		Texture2D texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(checkIDToPublish.playerOwner)) as Texture2D;
		auctionLine.transform.FindChild("Progress").FindChild("Chest").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		auctionLine.transform.FindChild("PlayerName").GetComponent<Text>().text = PlayerDatabase.instance.GetName(checkIDToPublish.playerOwner);
		auctionLine.transform.FindChild("Progress").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(checkIDToPublish.playerOwner);
		auctionLine.transform.FindChild("AuctionLineBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
		auctionLines.Add(auctionLine);
	}

	public void UpdateBidOnAuction(int index)
	{
		AuctionLineController auction = auctionLines[index].GetComponent<AuctionLineController>();
		auction.MoneyBidHasChanged(AuctionData.instance.bidsOnAuction[index], (float) AuctionData.instance.bidsOnAuction[index] * AuctionLineController.MAX_PROGRESS_BAR_WIDTH / 70f); // 70 = total money in game
	}

	public CheckID CheckToRevealed()
	{
		int moneyBidRef = 0;
		List<CheckID> winners = new List<CheckID>();
		List<GameObject> auctionLinesWinners = new List<GameObject>();
		foreach (GameObject auctionLine in auctionLines)
		{
			int index = auctionLine.GetComponent<AuctionLineController>().index;
			int tmpMoneyBid = AuctionData.instance.bidsOnAuction[index];
			if (tmpMoneyBid == moneyBidRef)
			{
				winners.Add(AuctionData.instance.checksIDToPublish[index]);
				auctionLinesWinners.Add(auctionLine);
				moneyBidRef = tmpMoneyBid;
			} else if (tmpMoneyBid > moneyBidRef)
			{
				winners = new List<CheckID>();
				winners.Add(AuctionData.instance.checksIDToPublish[index]);
				auctionLinesWinners = new List<GameObject>();
				auctionLinesWinners.Add(auctionLine);
				moneyBidRef = tmpMoneyBid;
			}
		}

		foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
		{
			foreach (GameObject a in auctionLinesWinners)
			{
				p.RpcUpdateMoneyAfterAuction(a.GetComponent<AuctionLineController>().index);
			}
		}

		winners.Shuffle();
		return winners.Count != 0 ? winners.First() : new CheckID();
	}

}
