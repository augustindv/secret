using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUpdate : MonoBehaviour {

    public GameObject inventoryLinePrefab;
    public GameObject inventorySecretPrefab;
    public GameObject inventorySecretEmptyPrefab;
    public GameObject inventoryContent;

    public void AddSecret(string playerNameTarget, string secretText, int imageID, bool newLine)
    {
        if (newLine)
        {
            GameObject line = Instantiate(inventoryLinePrefab, inventoryContent.transform);
            GameObject inventorySecret = Instantiate(inventorySecretPrefab, line.transform);
            GameObject inventorySecretEmpty = Instantiate(inventorySecretEmptyPrefab, line.transform);
            line.transform.localScale = new Vector3(1, 1, 1);
            inventorySecret.transform.localScale = new Vector3(1, 1, 1);
            inventorySecretEmpty.transform.localScale = new Vector3(1, 1, 1);
            Sprite sprite = PlayerDatabase.instance.GetSprite(playerNameTarget);
            inventorySecret.transform.FindChild("Holder").GetComponent<Image>().sprite = sprite;
            Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
            inventorySecret.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            inventorySecretEmpty.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            inventorySecret.GetComponent<Button>().onClick.AddListener(delegate { InventoryController.instance.OpenSecretPopup(playerNameTarget, secretText, imageID); });
        } else
        {
            GameObject line = inventoryContent.transform.GetChild(inventoryContent.transform.childCount - 1).gameObject;
            GameObject inventorySecretEmpty = line.transform.GetChild(line.transform.childCount - 1).gameObject;
            Destroy(inventorySecretEmpty);
            GameObject inventorySecret = Instantiate(inventorySecretPrefab, line.transform);
            line.transform.localScale = new Vector3(1, 1, 1);
            inventorySecret.transform.localScale = new Vector3(1, 1, 1);
            Sprite sprite = PlayerDatabase.instance.GetSprite(playerNameTarget);
            inventorySecret.transform.FindChild("Holder").GetComponent<Image>().sprite = sprite;
            Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
            inventorySecret.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            inventorySecret.GetComponent<Button>().onClick.AddListener(delegate { InventoryController.instance.OpenSecretPopup(playerNameTarget, secretText, imageID); });
        }
    }
}
