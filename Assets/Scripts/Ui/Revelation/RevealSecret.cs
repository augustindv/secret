using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealSecret : MonoBehaviour {

    public Text textSecret;

    public void UpdateSecretRevealed(string playerName, string secretText)
    {
        textSecret.text = "<color=" + PlayerDatabase.instance.GetColorHex(playerName) + ">" + playerName + "</color> " +  secretText;
    }
}
