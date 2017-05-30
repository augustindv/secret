using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Secret : NetworkBehaviour {

    [SyncVar]
    public bool shared;
    [SyncVar]
    public string secretTextProfile;
    [SyncVar]
    public string secretTextCommon;
    [SyncVar]
    public string secretTextClue;
    [SyncVar]
    public int secretID;

    public Sprite spriteIcon;

    public Secret(bool shared, string secretTextProfile, string secretTextCommon, int secretID)
    {
        this.shared = shared;
        this.secretTextProfile = secretTextProfile;
        this.secretTextCommon = secretTextCommon;
        this.secretID = secretID;
    }

}
