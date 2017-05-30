using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class DecisionBankEventLocalIn : UnityEvent<int>
{
}

[System.Serializable]
public class DecisionBankEventExternalIn : UnityEvent<int>
{
}

[System.Serializable]
public class DecisionBankEventLocalOut : UnityEvent<int>
{
}

[System.Serializable]
public class DecisionBankEventExternalOut : UnityEvent<int>
{
}

public class DecisionBankTarget : NetworkBehaviour {
    public DecisionBankEventLocalIn DecisionBankEventLocalEmitterIn;
    public DecisionBankEventExternalIn DecisionBankEventExternalEmitterIn;
    public DecisionBankEventLocalOut DecisionBankEventLocalEmitterOut;
    public DecisionBankEventExternalOut DecisionBankEventExternalEmitterOut;
    public int Count { get; set; }

    private void HandleEvent(Collider2D collision, bool isEnter)
    {
        string name = PlayerDatabase.instance.GetName(collision.transform.parent.GetComponent<NetworkIdentity>().netId);
        bool isLocalPlayer = name == PlayerDatabase.instance.PlayerName ? true : false;
        Count = Count + (isEnter ? 1 : -1);
        // TODO dirty fix
        //if(isEnter)
        {
            collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().TargetIsBank = isEnter;
            // collision.gameObject.transform.parent.gameObject.GetComponent<PlayerMarker>().Target = "none";
            Debug.Log("Decision bank: " + isEnter + " none ");
        }

        if (isLocalPlayer)
        {
            if(isEnter)
                DecisionBankEventLocalEmitterIn.Invoke(Count);
            else
                DecisionBankEventLocalEmitterOut.Invoke(Count);
        }
        else
        {
            if (isEnter)
                DecisionBankEventExternalEmitterIn.Invoke(Count);
            else
                DecisionBankEventExternalEmitterOut.Invoke(Count);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameDecisionController.instance.IsPlaying)
            HandleEvent(collision, true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if( GameDecisionController.instance.IsPlaying)
            HandleEvent(collision, false);
    }
}
