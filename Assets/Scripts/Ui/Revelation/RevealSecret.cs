using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealSecret : MonoBehaviour {

    public GameObject secretRevealed;

    public void UpdateSecretRevealed(string playerName, string secretText, int imageID)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        Transform secretIcon = secretRevealed.transform.FindChild("SecretIcon");
        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;
        Player p = PlayerDatabase.instance.GetPlayer(playerName);
        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
        secretRevealed.transform.FindChild("SecretText").GetComponent<Text>().text = playerName + " " + secretText;
        secretRevealed.transform.FindChild("PopUpBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
    }
}
