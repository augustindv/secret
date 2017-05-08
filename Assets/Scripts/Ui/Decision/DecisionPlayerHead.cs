using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionPlayerHead : MonoBehaviour {
    public void DecisionEvent(string id, bool isEnter, bool isLocalPlayer)
    {
        if(!isLocalPlayer)
        {
            if (isEnter)
                transform.localEulerAngles = new Vector3(0, 0, 20);
            else
                transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}
