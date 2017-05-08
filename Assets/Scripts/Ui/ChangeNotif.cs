using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNotif : MonoBehaviour {

    public Text notif;

    public void ChangeNotifText(string newText)
    {
        notif.text = newText.ToString();
    }
}
