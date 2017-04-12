﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct CheckID {

    public NetworkInstanceId playerOwner;
    public NetworkInstanceId playerTargeted;
    public NetworkInstanceId secret;

    public CheckID(NetworkInstanceId po, NetworkInstanceId p, NetworkInstanceId s)
    {
        playerOwner = po;
        playerTargeted = p;
        secret = s;
    }
}

public class SyncListCheck : SyncListStruct<CheckID> { }

public struct Check
{
    public Player playerOwner;
    public Player playerTargeted;
    public Secret secret;

    public Check(Player playerOwner, Player playerTargeted, Secret s)
    {
        this.playerOwner = playerOwner;
        this.playerTargeted = playerTargeted;
        secret = s;
    }
}