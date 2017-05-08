using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionPlayerBodyHighlight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DecisionEvent(string name, bool isEnter, bool isLocalPlayer)
    {
        if(isLocalPlayer)
            gameObject.SetActive(isEnter);
    }
}
