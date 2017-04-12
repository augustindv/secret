using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum GameStateSession
{
    Offline,
    Connecting,
    Lobby,
    Countdown,
    Running,
    GameOver
}

public class GameSession : NetworkBehaviour
{

    [SyncVar]
    public GameStateSession gameState;
    private Dropdown dropdown;

    public static GameSession instance;
    public static int NUMBER_TEAMS = 2;
    public static int NUMBER_SECRETS = 24;

    Listener networkListener;
    public List<Player> players;

    // Spawned objects
    GameObject sequencer;
    GameObject bank;
    List<GameObject> teams = new List<GameObject>();
    List<GameObject> secrets = new List<GameObject>();
    GameObject auctionData;


    void Start()
    {
        for (int i = 1; i <= NUMBER_TEAMS; i++)
        {
            GameObject team = Resources.Load<GameObject>("Teams/Team " + i);
            teams.Add(team);
            ClientScene.RegisterPrefab(team);
        }

        for (int i = 1; i <= NUMBER_SECRETS; i++)
        {
            GameObject secret = Resources.Load<GameObject>("Secrets/Secret "+i);
            secrets.Add(secret);
            ClientScene.RegisterPrefab(secret);
        }

        auctionData = Resources.Load<GameObject>("Auction/AuctionData");
        ClientScene.RegisterPrefab(auctionData);

        bank = Resources.Load<GameObject>("Bank");
        ClientScene.RegisterPrefab(bank);

        /*GameObject auctionLine = Resources.Load<GameObject>("Auction/AuctionLine");
        Debug.Log(auctionLine);
        ClientScene.RegisterPrefab(auctionLine);
        auctionLine = Instantiate(auctionLine);
        auctionLine.transform.parent = UiMainController.instance.uiAuction.transform.FindChild("Auction");*/
    }

    [Server]
    public override void OnStartServer()
    {
        networkListener = FindObjectOfType<Listener>();
        gameState = GameStateSession.Connecting;
    }

    [Server]
    public void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
    {
        players = aStartingPlayers.Select(p => p as Player).ToList();

        sequencer = Instantiate(Resources.Load<GameObject>("Sequencer"));
        bank = Instantiate(Resources.Load<GameObject>("Bank"));

        NetworkServer.Spawn(bank);

        auctionData = Instantiate(Resources.Load<GameObject>("Auction/AuctionData"));
        NetworkServer.Spawn(auctionData);

        CreateTeams();
        SpawnSecrets();

        RpcOnStartedGame();
        foreach (Player p in players)
        {
            p.RpcOnStartedGame();
        }

        StartCoroutine(RunGame());
    }

    [Server]
    public void OnAbortGame()
    {
        RpcOnAbortedGame();
    }

    [Server]
    public void CreateTeams()
    {
        for (int i = 0; i < teams.Count; i++)
        {
            teams[i] = Instantiate(teams[i]);
        }
        foreach (GameObject team in teams)
        {
            NetworkServer.Spawn(team);
        }
        teams.Shuffle();

        // Method to dispatch players : Take the list of players, shuffle it, slice it in two and dispatch to the 2 teams. (If more teams later : slice in 3, etc)
        int numberOfPlayers = players.Count;
        int numberOfPlayersByTeam = numberOfPlayers / NUMBER_TEAMS;
        List<Player> tmpPlayers = new List<Player>();
        tmpPlayers.AddRange(players);
        tmpPlayers.Shuffle(); // Always shuffle !

        for (int i = 1; i <= NUMBER_TEAMS; i++)
        {
            Team team = teams[i - 1].GetComponent<Team>();
            List<Player> listTeam = tmpPlayers.Take(numberOfPlayersByTeam).ToList<Player>();
            if (i == NUMBER_TEAMS)
            {
                // Put the rest of the players in the last team
                listTeam = tmpPlayers.Take(numberOfPlayers-(numberOfPlayersByTeam*(NUMBER_TEAMS-1))).ToList<Player>();
            }
            foreach (Player p in listTeam)
            {
                team.players.Add(p);
                p.teamId = team.netId;
                p.team = teams[i - 1];
                p.RpcSetTeam(team.netId);
                tmpPlayers.Remove(p);
            }
        }
    }

    [Server]
    public void SpawnSecrets()
    {
        for (int i = 0; i < secrets.Count; i++)
        {
            secrets[i] = Instantiate(secrets[i]);
        }

        TextAsset data = Resources.Load("Secrets/Secrets") as TextAsset;
        string[] lines = data.text.Split('\n');
        List<string> linesList = lines.ToList<string>();
        linesList.Shuffle();

        foreach (GameObject secret in secrets)
        {
            NetworkServer.Spawn(secret);
            secret.GetComponent<Secret>().secretText = linesList[0].Split(';')[0];
            secret.GetComponent<Secret>().imageID = int.Parse(linesList[0].Split(';')[1]);
            linesList.RemoveAt(0);
        }
        secrets.Shuffle();

        Invoke("DispatchSecrets", 1f);

    }

    [Server]
    public void DispatchSecrets()
    {
        // TODO : Something more proper and modulable to the number of players (have a problem with syncvar first to resolve)


        List<GameObject> tmpListSecrets = secrets.Take(secrets.Count / NUMBER_TEAMS).ToList();
        foreach (GameObject team in teams)
        {
            Team t = team.GetComponent<Team>();
            int numberOfSharedSecrets = t.players.Count == 1 ? 1 : t.players.Count - 1; // DEBUG : 1 to test with one player only (host test)...
            for (int i = 0; i < t.players.Count; i++)
            {
                Player p = t.players[i];
                for (int j = 0; j < numberOfSharedSecrets; j++)
                {
                    Player nextPlayer = t.players[(i + 1) % t.players.Count];
                    Secret secret = tmpListSecrets[0].GetComponent<Secret>();
                    secrets.Find(s => s.GetComponent<Secret>().Equals(secret)).GetComponent<Secret>().shared = true;
                    p.secrets.Add(new Structures.NetID() { netID = secret.netId });
                    if (!nextPlayer.playerName.Equals(p.playerName))
                    {
                        nextPlayer.secrets.Add(new Structures.NetID() { netID = secret.netId });
                    }
                    tmpListSecrets.Remove(secret.gameObject);
                }
                int tmp = 5 - p.secrets.Count;
                for (int j = 0; j < tmp; j++)
                {
                    Secret secret = tmpListSecrets[0].GetComponent<Secret>();
                    p.secrets.Add(new Structures.NetID() { netID = secret.netId });
                    tmpListSecrets.Remove(secret.gameObject);
                }
                p.secrets.Shuffle();
                p.RpcUpdateProfile();
            }
        }
    }

    [Client]
    public override void OnStartClient()
    {
        if (instance)
        {
            Debug.LogError("ERROR: Another GameSession!");
        }
        instance = this;
        networkListener = FindObjectOfType<Listener>();
        networkListener.gameSession = this;
        if (gameState != GameStateSession.Lobby)
        {
            gameState = GameStateSession.Lobby;
        }
    }

    public void OnJoinedLobby()
    {
        gameState = GameStateSession.Lobby;
    }

    public void OnLeftLobby()
    {
        gameState = GameStateSession.Offline;
    }

    public void OnCountdownStarted()
    {
        gameState = GameStateSession.Countdown;
    }

    public void OnCountdownCancelled()
    {
        gameState = GameStateSession.Lobby;
    }

    [Server]
    IEnumerator RunGame()
    {
        gameState = GameStateSession.Running;
        yield return null;
    }

    // Client RPCs

    [ClientRpc]
    public void RpcOnStartedGame()
    {
    }

    [ClientRpc]
    public void RpcOnAbortedGame()
    {
    }

    void Update()
    {

    }
}
