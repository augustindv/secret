using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    public int playerMoneyExtra = 0;
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
    public GamePhase gamePhase = GamePhase.Personnalisation;

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
    public void CmdChangeMoneyPlayer(int newMoney)
    {
        this.playerMoney = newMoney;
        // Obliged to do this to trigger event ?
        RpcChangeMoneyPlayer(newMoney);
    }

    [ClientRpc]
    public void RpcChangeMoneyPlayer(int newMoney)
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
        if (isLocalPlayer)
        {
            UiMainController.instance.localPlayer = this;
        }
    }

    public void ProfileSync()
    {
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

            PlayerDatabase.instance.PlayerName = playerName;

            CmdChangeMoneyPlayer(10);
            ColorController.instance.SetColoredUI(PlayerDatabase.instance.GetPlayerDeckID(this));

            Debug.Log("Dump on Profile Sync START");
            PlayerDatabase.instance.Dump();
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
    }

    public void SetPlayerName(string name)
    {
        CmdSetPlayerName(name);
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdSetPlayerCardDeck(int id)
    {
        cardDeck = id;
        RpcReadyNextPhase(true);
    }

    [ClientRpc]
    public void RpcReadyNextPhase(bool ready)
    {
        if (isLocalPlayer)
        {
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(ready);
        }
    }

    public void SetSelfieBytes(byte[] bytes)
    {
        if(isLocalPlayer)
        {
            CmdSetSelfieBytes(bytes, gameObject.GetComponent<NetworkIdentity>().netId);
            //CmdSetNextPhase(GamePhase.ScanDeck);
            CmdIsReadyForNextPhase(true);
        }
    }

    [Command]
    public void CmdSetSelfieBytes(byte[] bytes, NetworkInstanceId netId)
    {
        if(isServer)
        {
            Debug.Log(gameObject.GetComponent<NetworkIdentity>().netId + "received selfie for " + netId + " >>> " + bytes.Length);
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
            RpcSetPlayerSelfie(serverSelfie.image, netId);
        }
    }

    [ClientRpc]
    public void RpcSetPlayerSelfie(byte[] image, NetworkInstanceId netId)
    {
        if(isLocalPlayer)
        {
            Debug.Log("RpcSetTmpSelfie " + image);
            PlayerDatabase.instance.SetSelfie(netId, image);
            if(PlayerDatabase.instance.IsComplete())
            {
                Debug.Log("Dump on Profile Sync END");
                PlayerDatabase.instance.Dump();
                UpdateProfile();
                CmdIsReadyForNextPhase(true);
            }           
        }

    }

    public void UpdateProfile()
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
                checksID.Add(new CheckID(this.netId, targetedPlayerID, secretID, false, cardNumber));
                checks.Add(new Check(this, targetedPlayer, targetedSecret, false, cardNumber));
                InventoryController.instance.UpdateInventory(targetedSecret, targetedPlayer.playerName, false, cardNumber);
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

    [ClientRpc]
    public void RpcLaunchAnimation(GamePhase gamePhase)
    {
        if (isLocalPlayer)
        {
            UiMainController.instance.SetActiveAllCanvas(false);
            AnimationController.instance.PlayAnimationTransitionForPhase(gamePhase);
        }
    }

    [Command]
    public void CmdIsReadyForNextPhase(bool isReady)
    {
        Sequencer.instance.PlayerIsReadyForNextPhase(isReady);
    }


    [Command]
    public void CmdSetNextPhase(GamePhase nextPhase)
    {
        RpcStartPhase(nextPhase);
        if (Sequencer.gamePhase != nextPhase)
            Sequencer.gamePhase = nextPhase;
    }

    [ClientRpc]
    public void RpcStartPhase(GamePhase gamePhase)
    {
        if (isLocalPlayer)
        {
            selfie = StartGameController.instance.selfie.GetComponent<Selfie>();

            this.gamePhase = gamePhase;
            Debug.Log(">>> START PHASE " + gamePhase.ToString());
            switch (gamePhase)
            {
                case GamePhase.Personnalisation:
                    StartGameController.instance.StartPersonnalisation();
                    break;
                case GamePhase.ScanDeck:
                    if(VuforiaController.instance.skip == true)
                    {
                        CmdSetPlayerCardDeck(int.Parse(netId.ToString()));
                        UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
                    }
                    else
                        UiMainController.instance.uiVuforia.SetActive(true);
                    UiMainController.instance.uiStartGame.SetActive(false);
                    break;
                case GamePhase.ProfileSync:
                    ProfileSync();
                    break;
                case GamePhase.FirstDiscussion:
                    NotifController.instance.UpdateNotif();
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.uiVuforia.SetActive(false);
                    break;
                case GamePhase.DiscussionBeforeAuction:
                    UiMainController.instance.ResetUiMain();
                    UiMainController.instance.uiMain.SetActive(true);
                    if (GetComponent<PlayerMarker>().TargetIsBank == true) // TODO something cleaner
                        DecisionBankController.instance.StopPhase();
                    else
                        DecisionCheckController.instance.StopPhase();
                    UiMainController.instance.uiDecision.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(false);
                    break;
                case GamePhase.DiscussionBeforeDecision:
                    CmdIsGameEnded();
                    UiMainController.instance.ResetUiMain();
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.uiDecision.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiNothingPublished.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(false);
                    break;
                case GamePhase.Decision:
                    UiMainController.instance.uiDecision.SetActive(true);
                    GameDecisionController.instance.StartPhase();
                    UiMainController.instance.uiMain.SetActive(false);
                    CmdStartTimer(Sequencer.TIMER_DECISION);
                    break;
                case GamePhase.DecisionResult:
                    ProcessBank();
                    if (GetComponent<PlayerMarker>().TargetIsBank == true)
                        DecisionBankController.instance.StartPhase();
                    else
                        DecisionCheckController.instance.StartPhase(GetComponent<PlayerMarker>().Target);
                    UiMainController.instance.uiDecision.SetActive(false);
                    GameDecisionController.instance.StopPhase();
                    break;
                case GamePhase.Publishing:
                    CmdResetAuctionData();
                    AuctionController.instance.ResetUiAuction();
                    PublishingController.instance.ResetUiPublishing();
                    PublishingController.instance.UpdatePublishingPanel(this);
                    UiMainController.instance.uiMain.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(true);
                    break;
                case GamePhase.Auction:
                    if (checkToPublish.secret != null)
                    {
                        // TODO Some cleanup... I leave it like this just in case of a bug I would know about ;)
                        //CheckID checkIDToPublish = checksID.ToList().Find(c => checkToPublish.secret.netId.Equals(c.secret));
                        //CmdAddCheckToPublish(checkIDToPublish);
                    }
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(true);
                    CmdStartTimer(Sequencer.TIMER_AUCTION);
                    break;
                case GamePhase.NothingPublished:
                    CmdChangeMoneyPlayer(playerMoney - 1);
                    UiMainController.instance.SetActiveAllCanvas(false);
                    UiMainController.instance.uiNothingPublished.SetActive(true);
                    break;
                case GamePhase.Revelation:
                    if (isServer)
                    {
                        CmdUpdateRevealSecret();
                    }
                    if (AuctionData.instance.checksIDToPublish.Count == 1)
                    {
                        if (playerName != PlayerDatabase.instance.GetName(AuctionData.instance.checksIDToPublish[0].playerOwner))
                        {
                            CmdChangeMoneyPlayer(playerMoney - 1);
                        }
                    }
                    UiMainController.instance.uiRevelation.SetActive(true);
                    UiMainController.instance.uiAuction.SetActive(false);
                    break;
                case GamePhase.EndGame:
                    RpcLaunchAnimation(GamePhase.EndGame);
                    break;
            }
        }
    }

    [Command]
    public void CmdResetAuctionData()
    {
        if (isServer)
            AuctionData.instance.CmdResetAuctionData();
    }

    [Command]
    public void CmdStartTimer(int startTime)
    {
        if (isServer)
            Sequencer.instance.StartTimer(startTime);
    }

    [ClientRpc]
    public void RpcUpdateMoneyAfterAuction(int index)
    {
        if (isLocalPlayer)
        {
            int moneyBidByPlayer = AuctionController.instance.auctionLines[index].GetComponent<AuctionLineController>().moneyBidByLocalPlayer;
            CmdChangeMoneyPlayer(playerMoney - moneyBidByPlayer);
        }
    }

    [Command]
    public void CmdUpdateRevealSecret()
    {
        if (isServer)
        {
            CheckID checkToRevealed = AuctionController.instance.CheckToRevealed();

            if (checkToRevealed.playerOwner.Value != 0 && checkToRevealed.playerTargeted.Value != 0 && checkToRevealed.secret.Value != 0)
            {
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

                foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
                {
                    p.RpcUpdateRevealSecret(playerOwnerID, playerTargetedID, secretID, playerTargeted.playerName, secret.secretTextCommon, secret.secretID, checkToRevealed.cardID);
                }

                if (secret.shared)
                {
                    playerTargeted.team.GetComponent<Team>().hp -= 2;
                }
                else
                {
                    playerTargeted.team.GetComponent<Team>().hp--;
                }
            }
        }
    }

    [Command]
    public void CmdIsGameEnded()
    {
        if (isServer)
        {
            bool isGameEnded = false;
            foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
            {
                if (p.team.GetComponent<Team>().hp <= 0)
                {
                    isGameEnded = true;
                    break;
                }
            }
            if (isGameEnded)
            {
                foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
                {
                    p.CmdSetNextPhase(GamePhase.EndGame);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateRevealSecret(NetworkInstanceId playerOwnerID, NetworkInstanceId playerTargetedID, NetworkInstanceId secretID, string playerNameTargeted, string secretText, int imageID, int cardID)
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
            Check checkToAdd = new Check(PlayerDatabase.instance.GetPlayer(playerOwnerID), PlayerDatabase.instance.GetPlayer(playerTargetedID), secret, true, cardID);
            if (!playerName.Equals(playerNameTargeted) && !checks.Any(c => c.playerTargeted.Equals(checkToAdd.playerTargeted) && c.secret.Equals(checkToAdd.secret))) {
                checks.Add(checkToAdd);
                checksID.Add(new CheckID(playerOwnerID, playerTargetedID, secretID, true, cardID));
                InventoryController.instance.UpdateInventory(secret, playerNameTargeted, false, cardID);
            } else
            {
                Check checkPublished = checks.Find(c => c.playerTargeted.Equals(checkToAdd.playerTargeted) && c.secret.Equals(checkToAdd.secret));
                checks.Remove(checkPublished);
                checks.Add(checkToAdd);
                CheckID checkIDPublished = checksID.ToList().Find(c => c.playerTargeted.Equals(playerTargetedID) && c.secret.Equals(secretID));
                checksID.Remove(checkIDPublished);
                checksID.Add(new CheckID(playerOwnerID, playerTargetedID, secretID, true, cardID));
                UpdateAllInventory();
            }
        }
    }

    public void UpdateAllInventory()
    {
        InventoryController.instance.EmptyInventory();
        checks.ForEach(c => InventoryController.instance.UpdateInventory(c.secret, c.playerTargeted.playerName, c.published, c.cardID));
    }

    [Command]
    public void CmdAddCheckToPublish(CheckID checkIDToPublish)
    {
        if (isServer)
        {
            AuctionData.instance.checksIDToPublish.Add(checkIDToPublish);
            AuctionData.instance.bidsOnAuction.Add(0);
            foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
            {
                p.RpcAddAuctionLine(checkIDToPublish, AuctionData.instance.checksIDToPublish.Count - 1);
            }
        }
    }

    [ClientRpc]
    public void RpcAddAuctionLine(CheckID checkIDToPublish, int index)
    {
        if (isLocalPlayer)
        {
            Debug.Log("New check to publish : " + checkIDToPublish.playerOwner + " index " + index);
            AuctionController.instance.AddAuctionLine(checkIDToPublish, index);
        }
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

    public void SnapSelfie()
    {
        selfie.Snap(SetSelfieBytes);
    }

    void ProcessBank()
    {
        if (isServer)
        {
            int bankCount = 0;
            foreach (string name in PlayerDatabase.instance.GetAllPlayerNames())
            {
                if (PlayerDatabase.instance.GetPlayer(name).GetComponent<PlayerMarker>().TargetIsBank == true)
                    bankCount++;
            }
            if(bankCount > 0)
            {
                int money = Bank.instance.bankMoney / bankCount;
                Bank.instance.bankMoney -= money * bankCount;

                foreach (string name in PlayerDatabase.instance.GetAllPlayerNames())
                {
                    if (PlayerDatabase.instance.GetPlayer(name).GetComponent<PlayerMarker>().TargetIsBank == true)
                    {
                        PlayerDatabase.instance.GetPlayer(name).playerMoney += money;
                        PlayerDatabase.instance.GetPlayer(name).playerMoneyExtra = money;
                    }
                }
            }
        }
    }

    /*void OnGUI()
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

                    if (GUILayout.Button(IsReady() ? "Not ready" : "Ready", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
                    {
                        if (IsReady())
                        {
                            SendNotReadyToBeginMessage();
                        }
                        else
                        {
                            SendReadyToBeginMessage();
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
    }*/

    public void Ready()
    {
        if (IsReady())
        {
            SendNotReadyToBeginMessage();
        }
        else
        {
            SendReadyToBeginMessage();
        }
    }

    public void Start()
    {
        if (isLocalPlayer)
        {
            StartGameController.instance.Run();
            //StartGameController.instance.readyButton.onClick.AddListener(Ready);
        }
    }

    public void Update()
    {
        // Poor solution but works, see https://app.asana.com/0/323764028182496/324543590575070 for details
        if (gamePhase == GamePhase.Personnalisation)
        {
            Button button = StartGameController.instance.readyButton;
            GameSession gameSession = GameSession.instance;
            if (isLocalPlayer && !IsReady() && (gameSession.gameState == GameStateSession.Lobby) && button.gameObject.GetComponent<CanvasRenderer>().GetColor() == button.colors.pressedColor * button.colors.colorMultiplier)
            {
                SendReadyToBeginMessage();
                button.interactable = false;
            }
        }
    }

}
