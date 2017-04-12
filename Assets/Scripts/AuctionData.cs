using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

}
