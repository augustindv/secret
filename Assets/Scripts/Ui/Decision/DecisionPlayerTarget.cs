using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable]
public class DecisionEvent : UnityEvent<string, bool, bool>
{
}

[System.Serializable]
public class DecisionEventTest : UnityEvent<int>
{
}

public class DecisionPlayerTarget : MonoBehaviour {
    public DecisionEvent onDecisionEvent;
    public DecisionEventTest decisionEventEmitterEnter;
    public DecisionEventTest decisionEventEmitterExit;
    public string PlayerName { get; set; }

    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string name = PlayerDatabase.instance.GetName(collision.transform.parent.GetComponent<NetworkIdentity>().netId);
        bool isLocalPlayer = name == PlayerDatabase.instance.PlayerName ? true : false;
        onDecisionEvent.Invoke(name, true, isLocalPlayer);
        decisionEventEmitterEnter.Invoke(0);
        if (isLocalPlayer)
            collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().Target = name;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string name = PlayerDatabase.instance.GetName(collision.transform.parent.GetComponent<NetworkIdentity>().netId);
        bool isLocalPlayer = name == PlayerDatabase.instance.PlayerName ? true : false;
        onDecisionEvent.Invoke(name, false, isLocalPlayer);
        decisionEventEmitterExit.Invoke(1);
        if (isLocalPlayer)
            collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().Target = "none";
    }

    public void SetHead(Sprite sprite)
    {
        transform.FindChild("Head").gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
