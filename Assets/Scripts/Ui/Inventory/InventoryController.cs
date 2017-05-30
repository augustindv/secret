using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InventoryAddSecretUnityEvent : UnityEvent<string, string, string, string>
{
}

public class InventoryController : MonoBehaviour {

    public static string FOUND_EMPTY = "Vous n'avez récupéré aucun secret\npour le moment...";
    public static string REVEALED_EMPTY = "Aucun secret n'a été révélé\nau grand jour !";
    public InventoryAddSecretUnityEvent onInventoryUpdate;

    public delegate void InventoryUpdate(string playerNameTarget, string secretText, string imageCardID, string publishedAndShared);
    public event InventoryUpdate InventoryUpdated;

    public static InventoryController inventoryController;

    public static InventoryController instance
    {
        get
        {
            if (!inventoryController)
            {
                inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;

                if (!inventoryController)
                {
                    Debug.LogError("There needs to be one active InventoryController script on a GameObject in your scene.");
                } else
                {
                    inventoryController.Init();
                }

            }

            return inventoryController;
        }
    }

    public GameObject secretPopup;
    public Button readyButtonAuction;
    public Sprite notReadySprite;
    public Sprite readySprite;
    public GameObject secretsFound;
    public GameObject secretsRevealed;
    public GameObject secretsEmpty;

    void Init()
    {
        if (onInventoryUpdate == null)
        {
            onInventoryUpdate = new InventoryAddSecretUnityEvent();
        }
    }

    public void Start()
    {
        readyButtonAuction.onClick.AddListener(GoToAuction);
    }

    public void EmptyInventory()
    {
        if (secretsFound.transform.childCount == 2 && !secretsFound.transform.FindChild("InventorySecretsEmpty"))
        {
            GameObject empty = Instantiate(secretsEmpty, secretsFound.transform);
            empty.gameObject.name = "InventorySecretsEmpty";
            empty.transform.FindChild("Text").GetComponent<Text>().text = FOUND_EMPTY;
        }
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in secretsFound.transform) children.Add(child.gameObject);
        foreach (Transform child in secretsRevealed.transform) children.Add(child.gameObject);
        children.ForEach(child => {
            if (!child.name.Contains("InventorySecrets")) Destroy(child);
        });
    }

    public void ResetUiInventory()
    {
        readyButtonAuction.image.overrideSprite = readySprite;
    }

    public void UpdateInventory(Secret secret, string playerNameTarget, bool published, int cardID, bool shared)
    {
        string publishedTmp = published ? "1" : "0";
        string sharedTmp = shared ? "1" : "0";

        if (InventoryUpdated != null)
            InventoryUpdated(playerNameTarget, secret.secretTextCommon, secret.secretID + "," + cardID, publishedTmp + "," + sharedTmp);

        onInventoryUpdate.Invoke(playerNameTarget, secret.secretTextCommon, secret.secretID + "," + cardID, publishedTmp + "," + sharedTmp);
    }

    public void OpenSecretPopup(string playerNameTarget, string secretText, int imageID, bool shared)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        Transform secretIcon = secretPopup.transform.FindChild("SecretIcon");

        GameObject sharedText = secretPopup.transform.FindChild("Shared").gameObject;
        if (!shared)
        {
            sharedText.SetActive(false);
        }
        else
        {
            sharedText.SetActive(true);
        }

        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;

        Player p = PlayerDatabase.instance.GetPlayer(playerNameTarget);
        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
        secretPopup.transform.FindChild("SecretText").GetComponent<Text>().text = playerNameTarget + " " + secretText;
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.AddListener(delegate { secretPopup.SetActive(false); });
        secretPopup.transform.FindChild("PopUpBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
        secretPopup.SetActive(true);
    }

    public void GoToAuction()
    {
        if (readyButtonAuction.image.overrideSprite == readySprite)
        {
            readyButtonAuction.image.overrideSprite = notReadySprite;
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
            //UiMainController.instance.localPlayer.CmdStopTimer();
        }
        else
        {
            readyButtonAuction.image.overrideSprite = readySprite;
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(false);
        }
    }

}
