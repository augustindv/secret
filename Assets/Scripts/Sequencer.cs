using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    Personnalisation,
    ScanDeck,
    ProfileSync,
    FirstDiscussion,
    DiscussionBeforeDecision,
    DiscussionBeforeAuction,
    Decision,
    DecisionResult,
    Publishing,
    NothingPublished,
    Auction,
    Revelation,
    EndGame
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

    public static GamePhase gamePhase = GamePhase.Personnalisation;

    private int numberOfPlayersReady = 0;

    // Timer objects
    public static int TIMER_DECISION = 4;
    public static int TIMER_AUCTION = 4;
    public static int TIMER_MAIN = 60;

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
        LaunchAnimation();
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
            //GoToNextPhase();
            LaunchAnimation();
        }
    }

    public void LaunchAnimation()
    {
        gamePhase = GetNextPhase(gamePhase);
        foreach (Player p in GameSession.instance.players)
        {
            p.RpcLaunchAnimation(gamePhase);
        }
        numberOfPlayersReady = 0;
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
            case GamePhase.Personnalisation:
                return GamePhase.ScanDeck;
            case GamePhase.ScanDeck:
                return GamePhase.ProfileSync;
            case GamePhase.ProfileSync:
                return GamePhase.FirstDiscussion;
            case GamePhase.FirstDiscussion:
                return GamePhase.Decision;
            case GamePhase.DiscussionBeforeDecision:
                return GamePhase.Decision;
            case GamePhase.DiscussionBeforeAuction:
                return GamePhase.Publishing;
            case GamePhase.Decision:
                return GamePhase.DecisionResult;
            case GamePhase.DecisionResult:
                return GamePhase.DiscussionBeforeAuction;
            case GamePhase.Publishing:
                if (AuctionData.instance.checksIDToPublish.Count == 0)
                {
                    return GamePhase.NothingPublished;
                } else if (AuctionData.instance.checksIDToPublish.Count == 1)
                {
                    return GamePhase.Revelation;
                } else
                {
                    return GamePhase.Auction;
                }
            case GamePhase.NothingPublished:
                return GamePhase.DiscussionBeforeDecision;
            case GamePhase.Auction:
                return GamePhase.Revelation;
            case GamePhase.Revelation:
                return GamePhase.DiscussionBeforeDecision;
            default:
                return GamePhase.DiscussionBeforeDecision;
        }
    }
}
