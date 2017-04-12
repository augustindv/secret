using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class PlayerDatabase : MonoBehaviour {
    public static PlayerDatabase playerDatabase;
    private Dictionary<string, PlayerData> players = new Dictionary<string, PlayerData>();
    private Texture2D tex;
    public Texture2D mask;

    struct PlayerData
    {
        public Player player;
        public string name;
        public NetworkInstanceId netId;
        public Sprite sprite;
        public Texture2D texture;
        public int cardDeck;
    }

    public static PlayerDatabase instance
    {
        get
        {
            if (!playerDatabase)
            {
                playerDatabase = FindObjectOfType(typeof(PlayerDatabase)) as PlayerDatabase;

                if (!playerDatabase)
                {
                    Debug.LogError("There needs to be one active MessageController script on a GameObject in your scene.");
                }
                else
                {
                    playerDatabase.Init();
                }
            }

            return playerDatabase;
        }
    }

    void Init()
    {

    }

    public void AddPlayer(Player player, string name, NetworkInstanceId netId, int cardDeck)
    {
        if(players.ContainsKey(name) == false)
            players.Add(name, new PlayerData() { player = player, name = name, netId = netId, cardDeck = cardDeck});
    }

    public void SetSelfie(NetworkInstanceId netId, byte[] imageBytes)
    {
        // Load data into the texture.
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        byte[] tmp = texture.EncodeToPNG();
        texture.LoadImage(tmp);

        TextureMask.Mask(texture, mask);
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, Selfie.size, Selfie.size), new Vector2(0.5f, 0.5f), 100.0f);

        string name = "";
        int cardDeck = 0;
        Player player = null;
        foreach (var entry in players)
        {
            if(entry.Value.netId == netId)
            {
                name = entry.Value.name;
                cardDeck = entry.Value.cardDeck;
                player = entry.Value.player;
            }
        }
        if(name != "")
        {
            Debug.Log(name + "--" + netId);
            players.Remove(name);
            players.Add(name, new PlayerData() { player = player, name = name, netId = netId, sprite = sprite, texture = texture, cardDeck = cardDeck });
        }

        foreach (var entry in players)
        {
            if(entry.Value.sprite != null)
                Debug.Log("db>" + entry.Value.name + ">" + entry.Value.netId + ">" + entry.Value.sprite.rect);
            else
                Debug.Log("db>" + entry.Value.name + ">" + entry.Value.netId + ">none");
        }
    }

    public string[] GetAllPlayerNames()
    {
        string[] outputValue = new string[players.Count];
        int i = 0;
        foreach(PlayerData data in players.Values)
            outputValue[i++] = data.name;

        return outputValue;
    }

    public NetworkInstanceId GetNetId(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.netId;
    }

    public NetworkInstanceId GetNetId(int cardDeck)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.cardDeck == cardDeck);

        return playerData.netId;
    }

    public string GetName(NetworkInstanceId id)
    {

        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.netId == id);

        return playerData.name;
    }

    public Sprite GetSprite(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.sprite;
    }

    public int GetNumberOfPlayers()
    {
        return players.Count;
    }

    public List<Player> GetAllPlayers()
    {
        List<Player> outputValue = new List<Player>();
        foreach (PlayerData data in players.Values)
        {
            outputValue.Add(data.player);
        }

        return outputValue;
    }

    public Player GetPlayer(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.player;
    }

    public Player GetPlayer(NetworkInstanceId netId)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.netId == netId);

        return playerData.player;
    }

}
