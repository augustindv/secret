﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Listener : CaptainsMessListener {
    // Bien repris de l'exemple de CaptainsMess

    public enum NetworkState
    {
        Init,
        Offline,
        Connecting,
        Connected,
        Disrupted
    };

    [HideInInspector]
    public NetworkState networkState = NetworkState.Init;

    public GameObject gameSessionPrefab;
    public GameSession gameSession;

    // Use this for initialization
    void Start () {
        networkState = NetworkState.Offline;

        ClientScene.RegisterPrefab(gameSessionPrefab);
    }


    public override void OnStartConnecting()
    {
        networkState = NetworkState.Connecting;
    }

    public override void OnStopConnecting()
    {
        networkState = NetworkState.Offline;
    }

    public override void OnServerCreated()
    {
        // Create game session
        GameSession oldSession = FindObjectOfType<GameSession>();
        if (oldSession == null)
        {
            GameObject serverSession = Instantiate(gameSessionPrefab);
            NetworkServer.Spawn(serverSession);
        }
        else
        {
            Debug.LogError("GameSession already exists!");
        }
    }

    public override void OnJoinedLobby()
    {
        networkState = NetworkState.Connected;

        gameSession = FindObjectOfType<GameSession>();

        if (gameSession)
        {
            gameSession.OnJoinedLobby();
        }
    }

    public override void OnLeftLobby()
    {
        networkState = NetworkState.Offline;

        gameSession.OnLeftLobby();
    }

    public override void OnCountdownStarted()
    {
        gameSession = FindObjectOfType<GameSession>();

        gameSession.OnCountdownStarted();
    }

    public override void OnCountdownCancelled()
    {
        gameSession.OnCountdownCancelled();
    }

    public override void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
    {
        Debug.Log("GO!");
        gameSession.OnStartGame(aStartingPlayers);
    }

    public override void OnAbortGame()
    {
        Debug.Log("ABORT!");
        gameSession.OnAbortGame();
    }

}
