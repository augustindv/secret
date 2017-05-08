using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PlayerDatabase : MonoBehaviour {
    public static PlayerDatabase playerDatabase;
    private Dictionary<string, PlayerData> players = new Dictionary<string, PlayerData>();
    public Texture2D mask;
    public string PlayerName { get; set; }

    struct PlayerData
    {
        public Player player;
        public string name;
        public NetworkInstanceId netId;
        public Sprite sprite;
        public Texture2D colorTexture;
        public Texture2D chestTexture;
        public Texture2D texture;
        public int cardDeck;
        public Color deckColor;
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
            players.Add(name, new PlayerData() { sprite = null, player = player, name = name, netId = netId, cardDeck = cardDeck, deckColor = GetColor(cardDeck) });
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

        // For testing purposes, also write to a file in the project folder
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/Selfie" + cardDeck.ToString() + ".png", bytes);//"Assets/Resources/Selfies/Selfie" + cardDeck.ToString() + ".png", bytes);
        // For animations, TODO : check if it doesn't take too much time
        /*TextureImporter importer = AssetImporter.GetAtPath("Assets/Resources/Selfies/Selfie" + cardDeck.ToString() + ".png") as TextureImporter;
        //importer.isReadable = true;
        //importer.alphaIsTransparency = true;
        //importer.spritePivot = new Vector2(0.5f, 0.5f);
        importer.SaveAndReimport();
        sprite = Resources.Load<Sprite>("Selfies/Selfie" + netId.ToString());*/

        if (name != "")
        {
            Debug.Log(name + "--" + netId);
            players.Remove(name);
            Texture2D colorTexture = Resources.Load("Secrets/Colors/color-" + cardDeck) as Texture2D;
            Texture2D chestTexture = Resources.Load("Secrets/Chests/chest-" + cardDeck) as Texture2D;
            players.Add(name, new PlayerData() { player = player, name = name, netId = netId, sprite = sprite, colorTexture = colorTexture, chestTexture = chestTexture,
                texture = texture, cardDeck = cardDeck, deckColor = GetColor(cardDeck) });
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
        {
            listData.Add(data);
        }
        playerData = listData.Find(data => data.cardDeck == cardDeck);

        return playerData.netId;
    }

    // TODO optimize ???
    public string GetName(NetworkInstanceId netId)
    {
        string outputValue = "";

        foreach (PlayerData data in players.Values)
        {
            if (netId == data.netId)
                outputValue = data.name;
        }

        return outputValue;
    }

    /* public string GetName(NetworkInstanceId id)
    {

        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.netId == id);

        return playerData.name;
    }*/

    public Sprite GetSprite(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.sprite;
    }

    public Texture2D GetColorTexture(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.colorTexture;
    }

    public Texture2D GetChestTexture(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        return playerData.chestTexture;
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

    public bool IsComplete()
    {
        bool outputValue = true;
        string debugOut = "";

        // name and sprite should be sufficient to check
        foreach (PlayerData data in players.Values)
        {
            if (data.name == null || data.sprite == null)
            {
                outputValue = false;
                debugOut += ".";
            }
            else
            {
                debugOut += "X";
            }
        }

        Debug.Log("IsComplete status " + debugOut);
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

    public Color GetPlayerDeckColor(NetworkInstanceId netId)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.netId == netId);

        return playerData.deckColor;
    }

    public void Dump()
    {
        foreach (PlayerData data in players.Values)
            Debug.Log("Player Database:" + data.name + " id:" + data.netId + " sprite:" + data.sprite + " deck:" + data.cardDeck);
    }


    public Color GetPlayerDeckColor(Player player)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.player == player);

        return playerData.deckColor;
    }

    public int GetPlayerDeckID(Player player)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.player == player);

        return playerData.cardDeck;
    }

    public int GetPlayerDeckID(NetworkInstanceId netID)
    {
        PlayerData playerData;

        List<PlayerData> listData = new List<PlayerData>();
        foreach (PlayerData data in players.Values)
            listData.Add(data);
        playerData = listData.Find(data => data.netId == netID);

        return playerData.cardDeck;
    }

    public int GetPlayersCount()
    {
        return players.Count;
    }

    public Color GetColor(string name)
    {
        PlayerData playerData;

        players.TryGetValue(name, out playerData);

        switch (playerData.cardDeck)
        {
            case 1:
                return new Color(244f / 255f, 208f / 255f, 37f / 255f, 1f);
            case 2:
                return new Color(157f / 255f, 205f / 255f, 227f / 255f, 1f);
            case 3:
                return new Color(198f / 255f, 44f / 255f, 67f / 255f, 1f);
            case 4:
                return new Color(206f / 255f, 94f / 255f, 41f / 255f, 1f);
            case 5:
                return new Color(56f / 255f, 177f / 255f, 137f / 255f, 1f);
            case 6:
                return new Color(100f / 255f, 39f / 255f, 127f / 255f, 1f);
            default:
                return new Color(0, 0, 0, 1f);
        }
    }

    public Color GetColor(int deckID)
    {
        switch (deckID)
        {
            case 1:
                return new Color(244f / 255f, 208f / 255f, 37f / 255f, 1f);
            case 2:
                return new Color(157f / 255f, 205f / 255f, 227f / 255f, 1f);
            case 3:
                return new Color(198f / 255f, 44f / 255f, 67f / 255f, 1f);
            case 4:
                return new Color(206f / 255f, 94f / 255f, 41f / 255f, 1f);
            case 5:
                return new Color(56f / 255f, 177f / 255f, 137f / 255f, 1f);
            case 6:
                return new Color(100f / 255f, 39f / 255f, 127f / 255f, 1f);
            default:
                return new Color(0, 0, 0, 1f);
        }
    }
}
