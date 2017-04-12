using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ChangeTimerUnityEvent : UnityEvent<int>
{
}

public class TimerController : MonoBehaviour {

    public static TimerController decisionController;
    public static TimerController instance
    {
        get
        {
            if (!decisionController)
            {
                decisionController = FindObjectOfType(typeof(TimerController)) as TimerController;

                if (!decisionController)
                {
                    Debug.LogError("There needs to be one active DecisionController script on a GameObject in your scene.");
                }
            }

            return decisionController;
        }
    }

    public ChangeTimerUnityEvent onTimerChange;

    public delegate void ChangeTimer(int newTimer);
    public event ChangeTimer TimerChanged;

    public void TimerHasChanged(int newTimer)
    {
        if (TimerChanged != null)
            TimerChanged(newTimer);

        onTimerChange.Invoke(newTimer);
    }
}
