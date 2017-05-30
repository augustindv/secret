using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTimer : MonoBehaviour {

    public Text auctionTimer;
    public Text decisionTimer;
    public Text discussionTimer;

    public void TimerHasChanged(int newTimer)
    {
        auctionTimer.text = newTimer + "''";
        decisionTimer.text = newTimer + "''";
        discussionTimer.text = newTimer + "''";
    }
}
