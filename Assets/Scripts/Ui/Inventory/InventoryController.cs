using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InventoryAddSecretUnityEvent : UnityEvent<string, string, int, bool>
{
}

public class InventoryController : MonoBehaviour {

    public InventoryAddSecretUnityEvent onInventoryUpdate;

    public delegate void InventoryUpdate(string playerNameTarget, string secretText, int imageID, bool newLine);
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

    public void ResetUiInventory()
    {
        readyButtonAuction.image.overrideSprite = readySprite;
    }

    public void UpdateInventory(Secret secret, string playerNameTarget, bool newLine)
    {

        if (InventoryUpdated != null)
            InventoryUpdated(playerNameTarget, secret.secretText, secret.imageID, newLine);

        onInventoryUpdate.Invoke(playerNameTarget, secret.secretText, secret.imageID, newLine);
    }

    public void OpenSecretPopup(string playerNameTarget, string secretText, int imageID)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        secretPopup.transform.FindChild("SecretIcon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        secretPopup.transform.FindChild("SecretText").GetComponent<Text>().text = playerNameTarget + " " + secretText;
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.AddListener(delegate { secretPopup.SetActive(false); });
        secretPopup.SetActive(true);
    }

    public void GoToAuction()
    {
        if (readyButtonAuction.image.overrideSprite == readySprite)
        {
            readyButtonAuction.image.overrideSprite = notReadySprite;
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
        }
        else
        {
            readyButtonAuction.image.overrideSprite = readySprite;
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(false);
        }
    }

}
