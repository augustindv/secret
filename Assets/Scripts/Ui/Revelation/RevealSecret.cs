using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealSecret : MonoBehaviour {

    public GameObject secretRevealed;

    public void UpdateSecretRevealed(string playerName, string secretText, int imageID)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        secretRevealed.transform.FindChild("SecretIcon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        secretRevealed.transform.FindChild("SecretText").GetComponent<Text>().text = playerName + " " + secretText;
    }
}
