using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameDecisionController : MonoBehaviour
{
    private Vector3[] positions = new Vector3[8];
    public GameObject prefab;
    public GameObject gameDecision;
    private static GameDecisionController gameDecisionController;
    List<GameObject> playerObjects = new List<GameObject>();
    public bool IsPlaying { get; set; }

    // Use this for initialization
    void Start()
    {
        IsPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static GameDecisionController instance
    {
        get
        {
            if (!gameDecisionController)
            {
                gameDecisionController = FindObjectOfType(typeof(GameDecisionController)) as GameDecisionController;

                if (!gameDecisionController)
                {
                    Debug.LogError("There needs to be one active StartGameController script on a GameObject in your scene.");
                }
            }

            return gameDecisionController;
        }
    }

    public void StartPhase()
    {
        //TODO should be done only once...
        Transform spawnTransforms = GameObject.Find("SpawnPoints").transform.FindChild("PlayerCount" + PlayerDatabase.instance.GetPlayersCount());
        for (int i = 0; i < PlayerDatabase.instance.GetPlayersCount(); i++)
            positions[i] = spawnTransforms.transform.FindChild("Player" + i).position;

        string[] players = PlayerDatabase.instance.GetAllPlayerNames();

        for (int i = 0; i < players.Length; i++)
        {
            GameObject go = Instantiate(prefab);

            go.transform.SetParent(gameDecision.transform);
            go.transform.position = positions[i];
            go.GetComponent<DecisionPlayerTarget>().SetVisual(
                PlayerDatabase.instance.GetSprite(players[i]),
                PlayerDatabase.instance.GetChestSprite(players[i]),
                players[i],
                PlayerDatabase.instance.GetColor(players[i])
                );
            go.GetComponent<DecisionPlayerTarget>().PlayerName = players[i];
            playerObjects.Add(go);
            GameObject marker = PlayerDatabase.instance.GetPlayer(players[i]).transform.Find("DecisionMarker").gameObject;
            marker.SetActive(true);
            marker.transform.Find("NameDisplay").gameObject.GetComponent<TextMesh>().text = players[i].Substring(0,Mathf.Min(2, players[i].Length));
            marker.GetComponent<SpriteRenderer>().color = PlayerDatabase.instance.GetColor(players[i]);

            //PlayerDatabase.instance.GetPlayer(players[i]).GetComponent<PlayerMarker>().isDecisionRunning = true;
        }

        GameObject playerGameObject = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).gameObject;

        //playerGameObject.transform.Find("DecisionMarker").gameObject.SetActive(true);
        //playerGameObject.GetComponent<PlayerMarker>().isDecisionRunning = true;
        playerGameObject.GetComponent<PlayerMarker>().StartDecision();

        IsPlaying = true;
    }

    public void StopPhase()
    {
        IsPlaying = false;
        GameObject playerGameObject = PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).gameObject;

        //playerGameObject.transform.Find("DecisionMarker").gameObject.SetActive(false);
        //playerGameObject.GetComponent<PlayerMarker>().isDecisionRunning = false;
        playerGameObject.GetComponent<PlayerMarker>().StopDecision();

        playerObjects.RemoveAll(go => {
            Destroy(go);
            return true;
        });

        string[] players = PlayerDatabase.instance.GetAllPlayerNames();

        for (int i = 0; i < players.Length; i++)
        {
            PlayerDatabase.instance.GetPlayer(players[i]).transform.Find("DecisionMarker").gameObject.SetActive(false);
            //PlayerDatabase.instance.GetPlayer(players[i]).GetComponent<PlayerMarker>().isDecisionRunning = false;
        }
    }
}