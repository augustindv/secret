using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct CheckID {

    public NetworkInstanceId playerOwner;
    public NetworkInstanceId playerTargeted;
    public NetworkInstanceId secret;
    public bool published;
    public int cardID;

    public CheckID(NetworkInstanceId po, NetworkInstanceId p, NetworkInstanceId s, bool publi, int cID)
    {
        playerOwner = po;
        playerTargeted = p;
        secret = s;
        published = publi;
        cardID = cID;
    }
}

public class SyncListCheck : SyncListStruct<CheckID> { }

public struct Check
{
    public Player playerOwner;
    public Player playerTargeted;
    public Secret secret;
    public bool published;
    public int cardID;

    public Check(Player playerOwner, Player playerTargeted, Secret s, bool publi, int cID)
    {
        this.playerOwner = playerOwner;
        this.playerTargeted = playerTargeted;
        secret = s;
        published = publi;
        cardID = cID;
    }
}
