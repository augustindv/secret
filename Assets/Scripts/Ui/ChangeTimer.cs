using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTimer : MonoBehaviour {

    public Text timer;

    public void TimerHasChanged(int newTimer)
    {
        timer.text = newTimer.ToString();
    }
}
