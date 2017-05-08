using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTimer : MonoBehaviour {

    public Text auctionTimer;
    public Text decisionTimer;

    public void TimerHasChanged(int newTimer)
    {
        auctionTimer.text = newTimer.ToString();
        decisionTimer.text = newTimer.ToString();
    }
}
