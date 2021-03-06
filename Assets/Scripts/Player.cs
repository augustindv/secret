﻿using System.Collections;
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

    // last color value for ready button - vrey drity patch for audio
    Color lastReadyColor = new Color(1, 1, 1, 1);

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
        newMoney = newMoney < 0 ? 0 : newMoney;
        this.playerMoney = newMoney;
        // Obliged to do this to trigger event ?
        RpcChangeMoneyPlayer(newMoney);
    }

    [Command]
    public void CmdChangeMoneyBank(int newMoney)
    {
        Bank.instance.bankMoney = newMoney;
    }

    [Command]
    public void CmdChangeExtraMoneyPlayer(int newMoney)
    {
        newMoney = newMoney < 0 ? 0 : newMoney;
        this.playerMoneyExtra = newMoney;
        RpcSetExtraMoney(newMoney);
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

    List<Player> playersUpdate = new List<Player>();
    int playerUpdateIndex;
    
    [Command]
    void CmdPlayerUpdateFinished(NetworkInstanceId serverNetId)
    {
        Debug.Log("CmdPlayerUpdateFinished " + netId);
        if(isServer && isLocalPlayer)
        {
            Debug.LogWarning("On server Update Finished " + playerUpdateIndex);
            playersUpdate.ForEach(p => Debug.Log("netId " + p.netId));
            if (playerUpdateIndex < playersUpdate.Count)
            {
                Player targetPlayer = playersUpdate[playerUpdateIndex];
                int selfieCount = 0;
                int totalCount = playersUpdate.Count * playersUpdate.Count;
                FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(player =>
                {
                    (targetPlayer as Player).RpcAddPlayer((player as Player).playerName, (player as Player).netId, (player as Player).cardDeck);

                    ServerSelfie serverSelfie = serverSelfies.Find(s => s.netId == (player as Player).netId);
                    (targetPlayer as Player).RpcSetPlayerSelfieFromServer(serverSelfie.image, (player as Player).netId);

                    LoadIndicatorController.instance.setLoad(100 * ((playerUpdateIndex * playersUpdate.Count) + selfieCount++ ) / totalCount);
                });
                (targetPlayer as Player).RpcFinalizePlayer(netId);
                playerUpdateIndex++;
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                ClientScene.FindLocalObject(serverNetId).GetComponent<Player>().CmdPlayerUpdateFinished(serverNetId);
            }
            else if (isServer)
            {
                NetworkServer.FindLocalObject(serverNetId).GetComponent<Player>().CmdPlayerUpdateFinished(serverNetId);
            }
        }
    }

    public void ProfileSync()
    {
        if (isServer && isLocalPlayer)
        {
            FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(targetPlayer =>
            {
                Debug.LogWarning("profile sync adding netId " + targetPlayer.netId);
                playersUpdate.Add(targetPlayer as Player);
            });
        }

// TENTATIVE
/* if(isServer)
{

    FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(targetPlayer =>
    {
        playersUpdate.Add(targetPlayer as Player);
    });


        FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(targetPlayer =>
    {
        FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(player =>
        {
            (targetPlayer as Player).RpcAddPlayer((player as Player).playerName, (player as Player).netId, (player as Player).cardDeck);

            ServerSelfie serverSelfie = serverSelfies.Find(s => s.netId == (player as Player).netId);
            (targetPlayer as Player).RpcSetPlayerSelfieFromServer(serverSelfie.image, (player as Player).netId);
        });
        (targetPlayer as Player).RpcFinalizePlayer();
        Sequencer.instance.PlayerIsReadyForNextPhase(true);
    });
} */

        if (isServer && isLocalPlayer)
        {
            playerUpdateIndex = 0;
            CmdPlayerUpdateFinished(netId);
        }

        if (isLocalPlayer)
        {

            MessageController.instance.MessageSendActionRequest += new MessageController.MessageSendHandler(Echo);

            //StartCoroutine(AddAllPlayers(0.5f));
            /* TENTATIVE FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers().ForEach(p =>
            {
                PlayerDatabase.instance.AddPlayer(p as Player, (p as Player).playerName, (p as Player).netId, (p as Player).cardDeck);

                CmdGetSelfieBytes((p as Player).netId, netId);
            });

            MessageController.instance.PlayerName = playerName;

            PlayerDatabase.instance.PlayerName = playerName;

            MessageController.instance.GameStart();


            CmdChangeMoneyPlayer(10);
            ColorController.instance.SetColoredUI(PlayerDatabase.instance.GetPlayerDeckID(this));

            Debug.Log("Dump on Profile Sync START");
            PlayerDatabase.instance.Dump(); */
        }
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
        if (isLocalPlayer)
        {
            CmdSetSelfieBytes(bytes, gameObject.GetComponent<NetworkIdentity>().netId);
            CmdSetNextPhase(GamePhase.ScanDeck);
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
    public void CmdGetSelfieBytes(NetworkInstanceId netId, NetworkInstanceId targetNetId)
    {
        if(isServer)
        {
            StartCoroutine(SetPlayerSelfieDelayed(netId, targetNetId));
        }
    }

    IEnumerator SetPlayerSelfieDelayed(NetworkInstanceId netId, NetworkInstanceId targetNetId)
    {
        yield return new WaitForSeconds(targetNetId.Value * 6 * 0.1f + netId.Value * 0.1f);
        //serverSelfies.ForEach(s => Debug.Log("[" + s.netId + "]"));
        ServerSelfie serverSelfie = serverSelfies.Find(s => s.netId == netId);
        Debug.Log("CmdGetSelfieBytes delayed for " + targetNetId + "/" + netId); // + "-- >" + serverSelfie.image);

        RpcSetPlayerSelfie(serverSelfie.image, netId);
    }



    [ClientRpc]
    public void RpcSetPlayerSelfie(byte[] image, NetworkInstanceId netId)
    {
        if(isLocalPlayer)
        {
            Debug.Log("RpcSetTmpSelfie for " + netId);
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

    [ClientRpc]
    public void RpcSetPlayerSelfieFromServer(byte[] image, NetworkInstanceId netId)
    {
        if (isLocalPlayer)
        {
            Debug.Log("RpcSetTmpSelfie for " + netId);
            PlayerDatabase.instance.SetSelfie(netId, image);
        }
    }

    [ClientRpc]
    public void RpcFinalizePlayer(NetworkInstanceId serverNetId)
    {
        Debug.Log("RpcFinalizePlayer " + netId);
        if (isLocalPlayer)
        {
            Debug.LogWarning("RpcFinalizePlayer for local player");
            UpdateProfile();
            MessageController.instance.GameStart();
            CmdPlayerUpdateFinished(serverNetId);
            CmdIsReadyForNextPhase(true);
        }
    }


    [ClientRpc]
    public void RpcAddPlayer(string name, NetworkInstanceId netId, int cardDeck)
    {
        if (isLocalPlayer)
        {
            Debug.LogWarning("Adding " + name + "netId" + netId);
            Player player = ClientScene.FindLocalObject(netId).GetComponent<Player>();
            PlayerDatabase.instance.AddPlayer(player, name, netId, cardDeck);
            // TENTATIVE 
            if (player == this)
            {
                Debug.LogWarning("Hey that's me I am " + name + "netId" + netId);
                MoneyController.instance.MoneyHasChanged(10);
                ColorController.instance.SetColoredUI(PlayerDatabase.instance.GetPlayerDeckID(this));
                MessageController.instance.PlayerName = name;
                PlayerDatabase.instance.PlayerName = name;
            }
        }
    }

    [ClientRpc]
    public void RpcPlaySyncAudioForPhase(GamePhase gamePhase, float delay)
    {
        if (isLocalPlayer)
        {
                AudioController.instance.PlayLocalAudioForPhase(gamePhase, delay);
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
            if (!checks.ToList().Exists(st => st.playerTargeted == targetedPlayer && st.secret.secretID == targetedSecret.secretID))
            {
                checksID.Add(new CheckID(this.netId, targetedPlayerID, secretID, false, cardNumber));
                checks.Add(new Check(this, targetedPlayer, targetedSecret, false, cardNumber));
                InventoryController.instance.UpdateInventory(targetedSecret, targetedPlayer.playerName, false, cardNumber, targetedSecret.shared);
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
            //TODO FIXME neeeds callback;
            if(gamePhase == GamePhase.DecisionResult)
                GameDecisionController.instance.StopPhase();
            AnimationController.instance.PlayAnimationTransitionForPhase(gamePhase);
            AudioController.instance.PlayLocalAudioForAnimationPhase(gamePhase);
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
                    AudioController.instance.StopLocalFx("IntroZap");
                    AudioController.instance.PlayLocalFx("IntroOk");
                    AudioController.instance.PlayLocalAudioForPhase(GamePhase.Personnalisation, 0);
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
                    LoadIndicatorController.instance.Activate(true);
                    AudioController.instance.PlayLocalFx("IntroOk");
                    AnimationController.instance.PlayWaitingAnimation();
                    ProfileSync();
                    break;
                case GamePhase.FirstDiscussion:
                    LoadIndicatorController.instance.Activate(false);
                    AnimationController.instance.StopWaitingAnimation();
                    NotifController.instance.UpdateNotif();
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.uiVuforia.SetActive(false);
                    AudioController.instance.PlayLocalAmbient();
                    AudioController.instance.PlayLocalRandomFx(5);
                    if (isServer)
                        CmdStartTimer(Sequencer.TIMER_FIRST_DISCUSSION);
                    break;
                case GamePhase.DiscussionBeforeAuction:
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.ResetUiMain();
                    if (GetComponent<PlayerMarker>().TargetIsBank == true) // TODO something cleaner
                        DecisionBankController.instance.StopPhase();
                    else
                        DecisionCheckController.instance.StopPhase();
                    UiMainController.instance.uiDecision.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(false);
                    if (isServer)
                        CmdStartTimer(Sequencer.TIMER_DISCUSSION);
                    break;
                case GamePhase.IsGameEnded:
                    if (isServer)
                    {
                        CmdIsGameEnded();
                    }
                    AnimationController.instance.StopWaitingAnimation();
                    break;
                case GamePhase.DiscussionBeforeDecision:
                    AnimationController.instance.StopWaitingAnimation();
                    UiMainController.instance.uiMain.SetActive(true);
                    UiMainController.instance.ResetUiMain();
                    UiMainController.instance.uiDecision.SetActive(false);
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiNothingPublished.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(false);
                    UiMainController.instance.uiRevelation.SetActive(false);
                    AudioController.instance.PlayLocalAmbient();
                    if (isServer)
                        CmdStartTimer(Sequencer.TIMER_DISCUSSION);
                    break;  
                case GamePhase.Decision:
                    if(isServer)
                        AudioController.instance.SyncAudioForPhase(GamePhase.Decision);
                    VuforiaController.instance.associatedCard = null;
                    UiMainController.instance.uiDecision.SetActive(true);
                    GameDecisionController.instance.StartPhase();
                    UiMainController.instance.uiMain.SetActive(false);
                    if (isServer)
                    {
                        CmdStartTimer(Sequencer.TIMER_DECISION);
                    }
                    break;
                case GamePhase.DecisionResult:
                    Debug.Log("DecisionResult bank:" + GetComponent<PlayerMarker>().TargetIsBank + " target" + GetComponent<PlayerMarker>().Target);
                    ProcessBank();
                    GameDecisionController.instance.StopPhase();
                    if (GetComponent<PlayerMarker>().TargetIsBank == true)
                        DecisionBankController.instance.StartPhase();
                    else if (GetComponent<PlayerMarker>().Target != "none" && GetComponent<PlayerMarker>().Target != null) // TODO fix me why can it be null
                        DecisionCheckController.instance.StartPhase(GetComponent<PlayerMarker>().Target);
                    else
                        DecisionLoserController.instance.StartPhase();
                    UiMainController.instance.uiDecision.SetActive(false);
                    AudioController.instance.PlayLocalAmbient();
                    AudioController.instance.PlayLocalRandomFx(60);
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
                    if (isServer)
                        AudioController.instance.SyncAudioForPhase(GamePhase.Auction);
                    if (checkToPublish.secret != null)
                    {
                        // TODO Some cleanup... I leave it like this just in case of a bug I would know about ;)
                        //CheckID checkIDToPublish = checksID.ToList().Find(c => checkToPublish.secret.netId.Equals(c.secret));
                        //CmdAddCheckToPublish(checkIDToPublish);
                    }
                    UiMainController.instance.uiPublishing.SetActive(false);
                    UiMainController.instance.uiAuction.SetActive(true);
                    if (isServer)
                    {
                        CmdStartTimer(Sequencer.TIMER_AUCTION);
                    }
                    break;
                case GamePhase.NothingPublished:
                    CmdChangeMoneyPlayer(playerMoney - 1);
                    CmdChangeMoneyBank(Bank.instance.bankMoney + 1);
                    UiMainController.instance.SetActiveAllCanvas(false);
                    AnimationController.instance.PlayAnimationNothingPublished();
                    UiMainController.instance.uiNothingPublished.SetActive(true);
                    break;
                case GamePhase.Revelation:
                    if (isServer)
                        AudioController.instance.SyncAudioForPhase(GamePhase.Revelation);
                    if (isServer)
                    {
                        CmdUpdateRevealSecret();
                    }
                    if (AuctionData.instance.checksIDToPublish.Count == 1)
                    {
                        if (playerName != PlayerDatabase.instance.GetName(AuctionData.instance.checksIDToPublish[0].playerOwner))
                        {
                            CmdChangeMoneyPlayer(playerMoney - 1);
                            CmdChangeMoneyBank(Bank.instance.bankMoney + 1);
                        }
                    }
                    UiMainController.instance.uiRevelation.SetActive(true);
                    UiMainController.instance.uiAuction.SetActive(false);
                    break;
                case GamePhase.EndGame:
                    AudioController.instance.PlayLocalAudioForPhase(GamePhase.EndGame, 0);
                    if (isServer)
                        CmdStopTimer();
                    UiMainController.instance.ResetUiMain();
                    UiMainController.instance.SetActiveAllCanvas(false);
                    UiMainController.instance.uiEndGame.SetActive(true);
                    InventoryController.instance.readyButtonAuction.gameObject.SetActive(false);
                    AnimationController.instance.PlayAnimationTransitionForPhase(GamePhase.EndGame);
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

    [Command]
    public void CmdStopTimer()
    {
        if (isServer)
            Sequencer.instance.StopTimer();
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
            } else
            {
                foreach (Player p in PlayerDatabase.instance.GetAllPlayers())
                {
                    p.CmdSetNextPhase(GamePhase.DiscussionBeforeDecision);
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
                InventoryController.instance.UpdateInventory(secret, playerNameTargeted, true, cardID, secret.shared);
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
        checks.ForEach(c => InventoryController.instance.UpdateInventory(c.secret, c.playerTargeted.playerName, c.published, c.cardID, c.secret.shared));
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
                    Player currentPlayer = PlayerDatabase.instance.GetPlayer(name);
                    if (currentPlayer.GetComponent<PlayerMarker>().TargetIsBank == true)
                    {
                        currentPlayer.CmdChangeMoneyPlayer(playerMoney + money);
                        currentPlayer.CmdChangeExtraMoneyPlayer(money);
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void RpcSetExtraMoney(int money)
    {
        if (isLocalPlayer)
        {
            DecisionBankController.instance.SetExtraMoneyText(money);
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

    public void Start()
    {
        if (isLocalPlayer)
        {
            StartGameController.instance.Run();
        }
    }

    public void Update()
    {
        // Poor solution but works, see https://app.asana.com/0/323764028182496/324543590575070 for details
        if (gamePhase == GamePhase.Personnalisation)
        {
            Button button = StartGameController.instance.readyButton;
            GameSession gameSession = GameSession.instance;
            //Debug.Log(button.colors.highlightedColor + " - " + button.colors.colorMultiplier);
            if (isLocalPlayer && !IsReady() && (gameSession.gameState == GameStateSession.Lobby))
            {
                Color color = button.gameObject.GetComponent<CanvasRenderer>().GetColor();
                //Debug.Log(color);
                if (color.r < lastReadyColor.r && !AudioController.instance.IsFxPlaying("IntroZap"))
                {
                    Debug.Log("play");
                    AudioController.instance.PlayLocalFx("IntroZap");
                }
                else if (color.r > lastReadyColor.r && AudioController.instance.IsFxPlaying("IntroZap"))
                {
                    Debug.Log("stop");
                    AudioController.instance.StopLocalFx("IntroZap");
                }

                lastReadyColor = color;
                if (color == button.colors.pressedColor * button.colors.colorMultiplier)
                {
                    SendReadyToBeginMessage();
                    button.interactable = false;
                }
            }

        }
    }

}
