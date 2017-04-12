using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Secret : NetworkBehaviour {

    [SyncVar]
    public bool shared;
    [SyncVar]
    public string secretText;
    [SyncVar]
    public int card1Id;
    [SyncVar]
    public int card2Id;
    [SyncVar]
    public bool published;

    // TODO : Image ?
    [SyncVar]
    public int imageID;
}
