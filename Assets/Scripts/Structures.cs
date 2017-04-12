using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class Structures : object {

    public class SyncListNetworkInstanceId : SyncListStruct<NetID> { }

    public struct NetID
    {
        public NetworkInstanceId netID;
    }
}
