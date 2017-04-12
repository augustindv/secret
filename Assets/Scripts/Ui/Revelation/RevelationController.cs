using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SecretRevelationUnityEvent : UnityEvent<string, string, int>
{
}

public class RevelationController : MonoBehaviour {

    public static RevelationController revelationController;
    public static RevelationController instance
    {
        get
        {
            if (!revelationController)
            {
                revelationController = FindObjectOfType(typeof(RevelationController)) as RevelationController;

                if (!revelationController)
                {
                    Debug.LogError("There needs to be one active RevelationController script on a GameObject in your scene.");
                }
            }

            return revelationController;
        }
    }

    public Button GoToDiscussion;

    public void Start()
    {
        GoToDiscussion.onClick.AddListener(delegate {
            UiMainController.instance.localPlayer.CmdSetNextPhase(GamePhase.Discussion);
        });
    }

    public SecretRevelationUnityEvent onSecretRevelation;

    public delegate void RevealSecret(string playerName, string secretText, int imageID);
    public event RevealSecret SecretRevealed;

    public void SecretIsRevealed(string playerName, string secretText, int imageID)
    {
        if (SecretRevealed != null)
            SecretRevealed(playerName, secretText, imageID);

        onSecretRevelation.Invoke(playerName, secretText, imageID);
    }
}
