using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Team : NetworkBehaviour
{

    public string teamName;
    public List<Player> players;
    [SyncVar]
    public int hp = 5;

}
