using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public struct ServerSelfie
{
    public NetworkInstanceId netId;
    public byte[] image;
}

[System.Serializable]
public class Player : CaptainsMessPlayer {

    [SyncVar]
    public string playerName;
    [SyncVar]
    public NetworkInstanceId teamId;
    public GameObject team;
    [SyncVar]
    public int playerMoney = 10;
    [SyncVar]
    public int cardDeck;
    [SyncVar]
    public Structures.SyncListNetworkInstanceId cards = new Structures.SyncListNetworkInstanceId();
    [SyncVar]
    public Structures.SyncListNetworkInstanceId secrets = new Structures.SyncListNetworkInstanceId();
    [SyncVar]
    public SyncListCheck checksID = new SyncListCheck();

    public List<Check> checks = new List<Check>();
    public Check checkToPublish;

    // VVVVVVVVVV
    private Selfie selfie;
    public delegate void PictureReady(byte[] picture);
   // public delegate void ReturnSelfie(out byte[] picture);
    private byte[] selfieBytes;
    private byte[] tmpBytes;


    static private List<ServerSelfie> serverSelfies = new List<ServerSelfie>();

        /* public class SyncPlayerRefs : SyncListStruct<PlayerRef> { }
    public SyncPlayerRefs syncPlayerRefs = new SyncPlayerRefs();

    void SyncPlayerRefsChanged(SyncListStruct<PlayerRef>.Operation op, int itemIndex)
    {
        Debug.Log("SyncPlayerRef changed:" + op + " " + itemIndex);
        if (op == SyncListStruct<PlayerRef>.Operation.OP_ADD)
            Debug.Log("Added "+ syncPlayerRefs[itemIndex].name);
    } */
    // ^^^^^^^^^^

    void Awake()
    {
    }

    /* vvvvvvvvv */
    // coroutine that pushes all player data to the player database
    /* public IEnumerator AddAllPlayers(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Get all players and add then to the player database (needs a dirty downcast) 
        FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(p =>
            {
                PlayerDatabase.instance.AddPlayer((p as Player).playerName,
                (p as Player).netId);
            }
        );
        foreach(string name in PlayerDatabase.instance.GetAllPlayerNames())
        {
            Debug.Log("Player name " + name);
        }
        MessageController.instance.GameStart();
    } */

    [Command]
    void CmdChangeMoneyPlayer(int newMoney)
    {
        this.playerMoney = newMoney;
        // Obliged to do this to trigger event ?
        RpcChangeMoneyPlayer(newMoney);
    }

    [ClientRpc]
    void RpcChangeMoneyPlayer(int newMoney)
    {
        if (isLocalPlayer)
        {
            MoneyController.instance.MoneyHasChanged(newMoney);
        }
    }

    void Echo(string src, string dst, string message)
    {
        Debug.Log("Send request " + src + ">>" + dst + ":" + message);
        Debug.Log("src network id=" + gameObject.GetComponent<NetworkIdentity>().netId);
        CmdMessageSend(src, dst, message);
    }

    [Command]
    void CmdMessageSend(string src, string dst, string message)
    {
        //PlayerRef playerRef = FindObjectOfType<GameSession>().playerRefs.Find(p => p.name == dst);
        Debug.Log("CmdMessageSend>> " + dst + " @ " + NetworkServer.FindLocalObject(PlayerDatabase.instance.GetNetId(dst)));
        NetworkServer.FindLocalObject(PlayerDatabase.instance.GetNetId(dst)).GetComponent<Player>().RpcMessageReceive(src, dst, message);
    }

    [ClientRpc]
    public void RpcMessageReceive(string src, string dst, string message)
    {
        if(isLocalPlayer)
        {
            MessageController.instance.Receive(src, dst, message, true);
        }
    }
    /* ^^^^^^^^^ */

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        StartGameController.instance.SetActiveUIStartGame(false);

        /* vvvvvvvvv */
        if (isLocalPlayer)
        {
            MessageController.instance.MessageSendActionRequest += new MessageController.MessageSendHandler(Echo);

            //StartCoroutine(AddAllPlayers(0.5f));
            FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(p =>
            {
                PlayerDatabase.instance.AddPlayer(p as Player, (p as Player).playerName, (p as Player).netId, (p as Player).cardDeck);
                CmdGetSelfieBytes((p as Player).netId);
            });

            MessageController.instance.GameStart();

            MessageController.instance.PlayerName = playerName;

            CmdChangeMoneyPlayer(10);

            UiMainController.instance.localPlayer = this;
        }
        /* ^^^^^^^^^ */
    }

    [Command]
    public void CmdSendSelfie(NetworkInstanceId netId)
    {
    }

    [ClientRpc]
    public void RpcReturnSelfie(byte[] image)
    {
        if (!isLocalPlayer)
            return;

        tmpBytes = image;
    }


    [ClientRpc]
    public void RpcSetTeam(NetworkInstanceId id)
    {
        if (isLocalPlayer)
        {
            team = ClientScene.FindLocalObject(id);
            teamId = id;
        }
        else if (isServer)
        {
            team = NetworkServer.FindLocalObject(id);
            teamId = id;
        }
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        StartGameController.instance.SetActiveUIStartGame(true);

		selfie = GameObject.Find("Selfie").GetComponent<Selfie>();
    }

    // VVVVVVVVVVV
    public IEnumerator SetPlayerName(string name, string deckChoice, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        CmdSetPlayerName(name);
        CmdSetPlayerCardDeck(deckChoice);
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdSetPlayerCardDeck(string id)
    {
        cardDeck = int.Parse(id);
    }

    public void SetSelfieBytes(byte[] bytes)
    {
        if(isLocalPlayer)
            CmdSetSelfieBytes(bytes, gameObject.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    public void CmdSetSelfieBytes(byte[] bytes, NetworkInstanceId netId)
    {
        if(isServer)
        {
            Debug.Log(gameObject.GetComponent<NetworkIdentity>().netId + "received selfie for " + netId + " >>> " + bytes.Length);
            /* string deb = "";
            foreach (byte b in bytes)
                deb += b.ToString() + ",";
            Debug.Log("CmdSetSelfieBytes>" + deb);*/
            serverSelfies.Add(new ServerSelfie() { netId = netId, image = bytes });
            serverSelfies.ForEach(s => Debug.Log("[" + s.netId + "]"));
            Debug.Log("count " + serverSelfies.Count);
        }
    }

    [Command]
    public void CmdGetSelfieBytes(NetworkInstanceId netId)
    {
        if(isServer)
        {
            serverSelfies.ForEach(s => Debug.Log("[" + s.netId + "]"));
            ServerSelfie serverSelfie = serverSelfies.Find(s => s.netId == netId);
            Debug.Log("CmdGetSelfieBytes for " + netId + "-->" + serverSelfie.image);
             /* string deb = "";
            foreach (byte b in serverSelfie.image)
                deb += b.ToString() + ",";
            Debug.Log("CmdGetSelfieBytes>" + deb); */
            RpcSetPlayerSelfie(serverSelfie.image, netId);
        }
    }

    [ClientRpc]
    public void RpcSetPlayerSelfie(byte[] image, NetworkInstanceId netId)
    {
        if(isLocalPlayer)
        {
            Debug.Log("RpcSetTmpSelfie " + image);
            /* string deb = "";
            foreach (byte b in image)
                deb += b.ToString() + ",";
            Debug.Log("RpcSetPlayerSelfie>" + netId + ">" + deb); */
            PlayerDatabase.instance.SetSelfie(netId, image);
        }

    }

    [ClientRpc]
    public void RpcUpdateProfile()
    {
        if (isLocalPlayer)
        {
            ProfileController.instance.UpdateProfileTeam(this);
            ProfileController.instance.UpdateProfileSecrets(this);
        }
    }

    public void DiscoverSecret(int cardDeck, int cardNumber)
    {
        NetworkInstanceId targetedPlayerID = PlayerDatabase.instance.GetNetId(cardDeck);
        Player targetedPlayer = null;
        if (isLocalPlayer)
        {
            targetedPlayer = ClientScene.FindLocalObject(targetedPlayerID).GetComponent<Player>();
        }
        else if (isServer)
        {
            targetedPlayer = NetworkServer.FindLocalObject(targetedPlayerID).GetComponent<Player>();
        }

        if (targetedPlayer != null)
        {
            NetworkInstanceId secretID = targetedPlayer.secrets[cardNumber - 1].netID;
            Secret targetedSecret = null;
            if (isLocalPlayer)
            {
                targetedSecret = ClientScene.FindLocalObject(secretID).GetComponent<Secret>();
            }
            else if (isServer)
            {
                targetedSecret = NetworkServer.FindLocalObject(secretID).GetComponent<Secret>();
            }
            if (!checksID.ToList().Exists(st => st.secret.Equals(secretID)))
            {
                checksID.Add(new CheckID(this.netId, targetedPlayerID, secretID));
                checks.Add(new Check(this, targetedPlayer, targetedSecret));
                InventoryController.instance.UpdateInventory(targetedSecret, targetedPlayer.playerName, (checksID.Count - 1) % 2 == 0);
            } else
            {
                Debug.Log("Secret already in inventory for " + targetedPlayerID + " with cardDeck number " + cardDeck + " and card number " + cardNumber);
            }
        } else
        {
            Debug.Log("No player found for netID " + targetedPlayerID + " with cardDeck number " + cardDeck + " and card number " + cardNumber);
        }
    }

    [ClientRpc]
    public void RpcUpdateTimer(int newTimer)
    {
        if (isLocalPlayer)
        {
            TimerController.instance.TimerHasChanged(newTimer);
        }
    }

    [Command]
    public void CmdIsReadyForNextPhase(bool isReady)
    {
        Sequencer.instance.PlayerIsReadyForNextPhase(isReady);
    }

    [ClientRpc]
    public void RpcStartPhase(GamePhase gamePhase)
    {
        Debug.Log("gamephase");
        if (isLocalPlayer)
        {
            switch (gamePhase)
            {
                case GamePhase.Discussion:
                    UiMainController.instance.ResetUiMain();
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(false);
                    break;
                case GamePhase.Decision:
                    break;
                case GamePhase.Publishing:
                    PublishingController.instance.UpdatePublishingPanel(this);
                    UiMainController.instance.uiMain.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(true);
                    break;
                case GamePhase.Auction:
                    if (checkToPublish.secret != null)
                    {
                        CheckID checkIDToPublish = checksID.ToList().Find(c => checkToPublish.secret.netId.Equals(c.secret));
                        CmdAddCheckToPublish(checkIDToPublish);
                    }
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(true);
                    CmdStartTimer(Sequencer.TIMER_AUCTION);
                    break;
                case GamePhase.Revelation:
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(true);
                    CmdUpdateRevealSecret();
                    break;
            }
        }
    }

    [Command]
    public void CmdStartTimer(int startTime)
    {
        if (isServer)
            Sequencer.instance.StartTimer(startTime);
    }

    [Command]
    public void CmdUpdateRevealSecret()
    {
        if (isServer)
        {
            CheckID checkToRevealed = AuctionController.instance.CheckToRevealed();
            NetworkInstanceId playerOwnerID = checkToRevealed.playerOwner;
            NetworkInstanceId playerTargetedID = checkToRevealed.playerTargeted;
            NetworkInstanceId secretID = checkToRevealed.secret;

            Player playerOwner = PlayerDatabase.instance.GetPlayer(playerOwnerID);
            Player playerTargeted = PlayerDatabase.instance.GetPlayer(playerTargetedID);

            Secret secret = null;
            if (isLocalPlayer)
            {
                secret = ClientScene.FindLocalObject(secretID).GetComponent<Secret>();
            }
            else if (isServer)
            {
                secret = NetworkServer.FindLocalObject(secretID).GetComponent<Secret>();
            }

            secret.published = true;
            playerTargeted.team.GetComponent<Team>().hp--;

            foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
            {
                p.RpcUpdateRevealSecret(playerOwnerID, playerTargetedID, secretID, playerTargeted.playerName, secret.secretText, secret.imageID);
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateRevealSecret(NetworkInstanceId playerOwnerID, NetworkInstanceId playerTargetedID, NetworkInstanceId secretID, string playerNameTargeted, string secretText, int imageID)
    {
        if (isLocalPlayer)
        {
            RevelationController.instance.SecretIsRevealed(playerNameTargeted, secretText, imageID);
            Secret secret = null;
            if (isLocalPlayer)
            {
                secret = ClientScene.FindLocalObject(secretID).GetComponent<Secret>();
            }
            else if (isServer)
            {
                secret = NetworkServer.FindLocalObject(secretID).GetComponent<Secret>();
            }
            Check checkToAdd = new Check(PlayerDatabase.instance.GetPlayer(playerOwnerID), PlayerDatabase.instance.GetPlayer(playerTargetedID), secret);
            Debug.Log(checkToAdd);
            if (!playerName.Equals(playerNameTargeted) && !checks.Any(c => c.playerTargeted.Equals(checkToAdd.playerTargeted) && c.secret.Equals(checkToAdd.secret))) {
                checks.Add(checkToAdd);
                checksID.Add(new CheckID(playerOwnerID, playerTargetedID, secretID));
                InventoryController.instance.UpdateInventory(secret, playerNameTargeted, (checksID.Count - 1) % 2 == 0);
            }
        }
    }

    [Command]
    public void CmdAddCheckToPublish(CheckID checkIDToPublish)
    {
        if (isServer)
        {
            AuctionData.instance.checksIDToPublish.Add(checkIDToPublish);
            RpcAddAuctionLine(checkIDToPublish);
        }
    }

    [ClientRpc]
    public void RpcAddAuctionLine(CheckID checkIDToPublish)
    {
        AuctionController.instance.AddAuctionLine(checkIDToPublish);
    }

    [ClientRpc]
    public void RpcUpdateBidOnAuction(int index)
    {
        if (isLocalPlayer)
            AuctionController.instance.UpdateBidOnAuction(index);
    }

    [Command]
    public void CmdUpdateBidOnAuction(int index)
    {
        foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
        {
            p.RpcUpdateBidOnAuction(index);
        }
    }

    [Command]
    public void CmdSetNextPhase(GamePhase nextPhase)
    {
        RpcStartPhase(nextPhase);
        if (Sequencer.gamePhase != nextPhase)
            Sequencer.gamePhase = nextPhase;
    }

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(0, Screen.height * 0.8f, Screen.width, 100));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GameSession gameSession = GameSession.instance;
            if (gameSession)
            {
                if (gameSession.gameState == GameStateSession.Lobby ||
                    gameSession.gameState == GameStateSession.Countdown)
                {
                    
                    if (StartGameController.instance.nameField.text.Length != 0 && GUILayout.Button(IsReady() ? "Not ready" : "Ready", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
                    {
                        if (IsReady())
                        {
                            SendNotReadyToBeginMessage();
                            StartGameController.instance.nameField.interactable = true;
                        }
                        else
                        {
                            SendReadyToBeginMessage();
                            StartGameController.instance.nameField.interactable = false;
                            // Delay time to set player name
                            Dropdown deckChoice = StartGameController.instance.deckChoice;
                            StartCoroutine(SetPlayerName(StartGameController.instance.nameField.text, deckChoice.options[deckChoice.value].text, 2f));
							selfie.Snap(SetSelfieBytes); // VVVVVVVVV
                        }
                    }
                }
                else if (gameSession.gameState == GameStateSession.Running)
                {

                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    void Update()
    {
    }

}
