using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUpdate : MonoBehaviour {

    public GameObject inventorySecretPrefab;
    public GameObject secretsFound;
    public GameObject secretsRevealed;

    public void AddSecret(string playerNameTarget, string secretText, string imageCardID, bool published)
    {
        int imageID = int.Parse(imageCardID.Split(',')[0]);
        string cardID = imageCardID.Split(',')[1];
        Color playerColor = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
        GameObject inventorySecret = null;
        if (!published)
        {
            inventorySecret = Instantiate(inventorySecretPrefab, secretsFound.transform);
            Transform emptyTmp = secretsFound.transform.FindChild("InventorySecretsEmpty");
            if (emptyTmp)
            {
                Destroy(emptyTmp.gameObject);
            }
        } else
        {
            inventorySecret = Instantiate(inventorySecretPrefab, secretsRevealed.transform);
            Transform emptyTmp = secretsRevealed.transform.FindChild("InventorySecretsEmpty");
            if (emptyTmp)
            {
                Destroy(emptyTmp.gameObject);
            }
        }

        inventorySecret.transform.localScale = new Vector3(1, 1, 1);
        inventorySecret.transform.FindChild("TextSecret").GetComponent<Text>().text = "<b>" + playerNameTarget + "</b> " + secretText;
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        Transform secretIcon = inventorySecret.transform.FindChild("Image");
        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;
        Player p = PlayerDatabase.instance.GetPlayer(playerNameTarget);
        secretIcon.FindChild("Icon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
        inventorySecret.transform.FindChild("SecretBg").GetComponent<Image>().color = published ? playerColor : new Color(24f / 255f, 26f / 255f, 32f / 255f, 1f);
        inventorySecret.transform.FindChild("SecretBorder").GetComponent<Image>().color = playerColor;
        inventorySecret.transform.FindChild("SecretNumber").GetComponent<Image>().color = playerColor;
        inventorySecret.transform.FindChild("SecretNumber").FindChild("Text").GetComponent<Text>().text = cardID;
        inventorySecret.GetComponent<Button>().onClick.AddListener(delegate { InventoryController.instance.OpenSecretPopup(playerNameTarget, secretText, imageID); });

    }
}
