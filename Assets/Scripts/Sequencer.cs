using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    Discussion,
    Decision,
    Publishing,
    Auction,
    Revelation,
    PhaseAnimation
}

public class Sequencer : MonoBehaviour {

    public static Sequencer sequencer;
    public static Sequencer instance
    {
        get
        {
            if (!sequencer)
            {
                sequencer = FindObjectOfType(typeof(Sequencer)) as Sequencer;

                if (!sequencer)
                {
                    Debug.LogError("There needs to be one active Sequencer script on a GameObject in your scene.");
                }
            }

            return sequencer;
        }
    }

    public static GamePhase gamePhase = GamePhase.Discussion;

    private int numberOfPlayersReady = 0;

    // Timer objects
    public static int TIMER_DECISION = 5;
    public static int TIMER_AUCTION = 5;

    public int timeInSeconds = TIMER_DECISION;
    public float time = 0;
    public bool timerRunning = false;

	void FixedUpdate () {
        if (timerRunning)
        {
            time += Time.fixedDeltaTime;
            if (time >= 1 && timeInSeconds != 0)
            {
                timeInSeconds -= 1;
                UpdateTimerOnPlayers(timeInSeconds);
                time = 0;
            } else if (timeInSeconds <= 0)
            {
                StopTimer();
            }

        }
	}

    public void StartTimer(int startTime)
    {
        timerRunning = true;
        timeInSeconds = startTime;
        UpdateTimerOnPlayers(timeInSeconds);
    }

    public void StopTimer()
    {
        timerRunning = false;
        gamePhase = GetNextPhase(gamePhase);
        foreach (Player p in GameSession.instance.players)
        {
            p.RpcStartPhase(gamePhase);
        }
        numberOfPlayersReady = 0;
    }

    public void UpdateTimerOnPlayers(int newTimer)
    {
        foreach (Player p in GameSession.instance.players)
        {
            p.RpcUpdateTimer(newTimer);
        }
    }

    public void PlayerIsReadyForNextPhase(bool isReady)
    {
        if (isReady)
        {
            numberOfPlayersReady++;
        } else if (numberOfPlayersReady > 0)
        {
            numberOfPlayersReady--;
        }
        if (AllPlayersReady())
        {
            GoToNextPhase();
        }
    }

    public void GoToNextPhase()
    {
        gamePhase = GetNextPhase(gamePhase);
        foreach (Player p in GameSession.instance.players)
        {
            p.RpcStartPhase(gamePhase);
        }
        numberOfPlayersReady = 0;
    }

    public bool AllPlayersReady()
    {
        return numberOfPlayersReady == GameSession.instance.players.Count;
    }

    public GamePhase GetNextPhase(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Discussion:
                return GamePhase.Publishing;
            case GamePhase.Decision:
                return GamePhase.Discussion;
            case GamePhase.Publishing:
                return GamePhase.Auction;
            case GamePhase.Auction:
                return GamePhase.Revelation;
            case GamePhase.Revelation:
                return GamePhase.Discussion;
            default:
                return GamePhase.Discussion;
        }
    }
}
