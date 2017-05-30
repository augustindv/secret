using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public AudioSource decision;
    public AudioSource auction;
    public AudioSource revelation;
    public AudioSource intro;
    public AudioSource introZap;
    public AudioSource introOk;
    public AudioSource scanOk;
    public AudioSource introPhoto;
    public AudioSource decisionAnimation;
    public AudioSource auctionAnimation;
    public AudioSource revelationAnimation;
    public AudioSource endGame;
    public AudioSource nothingPublished;
    public AudioSource personalization;
    public AudioSource genericFail;
    public AudioSource decisionBank;
    public AudioSource decisionCheck;
    public AudioSource randomFx;
    public List<AudioSource> ambients;
    public List<AudioClip> randomFxs;
    private Dictionary<GamePhase, AudioSource> audios = new Dictionary<GamePhase, AudioSource>();
    private Dictionary<GamePhase, AudioSource> animationAudios = new Dictionary<GamePhase, AudioSource>();
    private Dictionary<string, AudioSource> fxs = new Dictionary<string, AudioSource>();
    private static AudioController audioController;
    private AudioSource currentAudio;

    public static AudioController instance
    {
        get
        {
            if (!audioController)
            {
                audioController = FindObjectOfType(typeof(AudioController)) as AudioController;

                if (!audioController)
                {
                    Debug.LogError("There needs to be one active AudioController script on a GameObject in your scene.");
                }

            }

            return audioController;
        }
    }

    void Start()
    {
        audios.Add(GamePhase.Decision, decision);
        audios.Add(GamePhase.Auction, auction);
        audios.Add(GamePhase.Revelation, revelation);
        audios.Add(GamePhase.Personnalisation, personalization);
        audios.Add(GamePhase.EndGame, endGame);
        animationAudios.Add(GamePhase.Decision, decisionAnimation);
        animationAudios.Add(GamePhase.Auction, auctionAnimation);
        animationAudios.Add(GamePhase.Revelation, revelationAnimation);
        animationAudios.Add(GamePhase.NothingPublished, nothingPublished);

        fxs.Add("IntroZap", introZap);
        fxs.Add("IntroOk", introOk);
        fxs.Add("ScanOk", scanOk);
        fxs.Add("IntroPhoto", introPhoto);
        fxs.Add("GenericFail", genericFail);
        fxs.Add("DecisionCheck", decisionCheck);
        fxs.Add("DecisionBank", decisionBank);
        intro.Play();
        currentAudio = intro;
    }

    public void PlayLocalAudioForPhase(GamePhase gamePhase, float delay)
    {
        AudioSource audio;
        audios.TryGetValue(gamePhase, out audio);

        if (audio)
        {
            if (currentAudio && currentAudio.isPlaying)
                currentAudio.Stop();
            currentAudio = audio;
            audio.PlayDelayed(delay);
        }
    }

    public void PlayLocalAmbient()
    {
        AudioSource audio;

        audio = ambients[Random.Range(0, ambients.Count)];

        if (audio)
        {
            if (currentAudio && currentAudio.isPlaying)
                currentAudio.Stop();
            currentAudio = audio;
            audio.Play();
        }
    }

    public void PlayLocalRandomFx(float delayMax)
    {
        randomFx.clip = randomFxs[Random.Range(0, randomFxs.Count)];

        if (randomFx.clip)
        {
            randomFx.PlayDelayed(Random.Range(0,delayMax));
        }
    }

    public void PlayLocalAudioForAnimationPhase(GamePhase gamePhase)
    {
        AudioSource audio;
        animationAudios.TryGetValue(gamePhase, out audio);

        if (audio)
        {
            if (currentAudio && currentAudio.isPlaying)
                currentAudio.Stop();
            currentAudio = audio;
            audio.Play();
        }
    }

    public void PlayLocalFx(string name)
    {
        AudioSource audio;
        fxs.TryGetValue(name, out audio);

        if (audio)
        {
            audio.Play();
        }
    }

    public bool IsFxPlaying(string name)
    {
        AudioSource audio;
        fxs.TryGetValue(name, out audio);

        if (audio)
        {
            return audio.isPlaying;
        }
        return false;
    }

    public void StopLocalFx(string name)
    {
        AudioSource audio;
        fxs.TryGetValue(name, out audio);

        if (audio && audio.isPlaying)
        {
            audio.Stop();
        }
    }

    public void SyncAudioForPhase(GamePhase gamePhase)
    {
        float stepDelay = 0.04f;
        // float delay = (GameSession.instance.players.Count - 1 ) * stepDelay;
        foreach (Player p in GameSession.instance.players)
        {
            Debug.Log(p.playerName);

            p.RpcPlaySyncAudioForPhase(gamePhase, 0);
           // delay -= stepDelay;
        }
    }
}