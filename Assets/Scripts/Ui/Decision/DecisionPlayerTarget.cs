using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        string otherPlayerName = PlayerDatabase.instance.GetName(other.transform.parent.GetComponent<NetworkIdentity>().netId);
        bool isLocalPlayer = otherPlayerName == PlayerDatabase.instance.PlayerName ? true : false;
        onDecisionEvent.Invoke(otherPlayerName, true, isLocalPlayer);
        if(isLocalPlayer)
            decisionEventEmitterEnter.Invoke(0);
        if (isLocalPlayer && GameDecisionController.instance.IsPlaying)
        {
            // TODO dirty fix
            Debug.Log("Decision bank: false " + PlayerName);
            other.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().TargetIsBank = false;
            other.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().Target = PlayerName;
        }
  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string name = PlayerDatabase.instance.GetName(collision.transform.parent.GetComponent<NetworkIdentity>().netId);
        bool isLocalPlayer = name == PlayerDatabase.instance.PlayerName ? true : false;
        onDecisionEvent.Invoke(name, false, isLocalPlayer);
        if (isLocalPlayer)
            decisionEventEmitterExit.Invoke(1);
        if (isLocalPlayer && GameDecisionController.instance.IsPlaying)
        {
            // TODO dirty fix
            Debug.Log("Decision not applied bank:" +
                collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().TargetIsBank + " none");
            // collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().Target = "none";
        }
    }

    public void SetVisual(Sprite sprite, Sprite body, string name, Color color)
    {
        transform.FindChild("Head").gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        transform.FindChild("Body").gameObject.GetComponent<SpriteRenderer>().sprite = body;
        transform.FindChild("Name").gameObject.GetComponent<TextMesh>().text = name;
        transform.FindChild("Circle").gameObject.GetComponent<SpriteRenderer>().color = color;
    }
}
