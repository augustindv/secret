using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public struct Auction
{
    public NetworkInstanceId netIDPlayer;
    public int bid;
    public int auctionLineIndex;
}

public class SyncListAuction : SyncListStruct<Auction> { }

public class AuctionData : NetworkBehaviour {

    public static AuctionData auctionData;
    public static AuctionData instance
    {
        get
        {
            if (!auctionData)
            {
                auctionData = FindObjectOfType(typeof(AuctionData)) as AuctionData;

                if (!auctionData)
                {
                    Debug.LogError("There needs to be one active AuctionData script on a GameObject in your scene.");
                }
            }

            return auctionData;
        }
    }

    [SyncVar]
    public SyncListCheck checksIDToPublish = new SyncListCheck();
    [SyncVar]
    public SyncListInt bidsOnAuction = new SyncListInt();

    [Command]
    public void CmdResetAuctionData()
    {
        checksIDToPublish.Clear();
        bidsOnAuction.Clear();
    }

}
