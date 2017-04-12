using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//FindObjectOfType<CaptainsMessClient>().networkManager.LobbyPlayers();
// Unity event for messages 
[System.Serializable]
public class MessageUnityEvent : UnityEvent<string, string, string, bool>
{
}

[System.Serializable]
public class MessageAddDstUnityEvent : UnityEvent<string>
{
}

// Message controller: singleton for controller for messages in UI
public class MessageController : MonoBehaviour {
    public MessageUnityEvent onMessageSend;
    public MessageUnityEvent onMessageReceive;
    public MessageAddDstUnityEvent onMessageAddDst;
    public static MessageController messageController;

    public delegate void MessageSendHandler(string src, string dst, string message);
    public event MessageSendHandler MessageSendActionRequest;

    public string PlayerName { get; set; }

    public static MessageController instance
    {
        get
        {
            if (!messageController)
            {
                messageController = FindObjectOfType(typeof(MessageController)) as MessageController;

                if (!messageController)
                {
                    Debug.LogError("There needs to be one active MessageController script on a GameObject in your scene.");
                }
                else
                {
                    messageController.Init();
                }
            }

            return messageController;
        }
    }

    void Init()
    {
        if (onMessageSend == null)
            onMessageSend = new MessageUnityEvent();

        onMessageSend.AddListener(Echo);
    }

    public void GameStart()
    {
        UiMainController.instance.uiMain.SetActive(true);
        foreach (string name in PlayerDatabase.instance.GetAllPlayerNames())
        {
            Debug.Log("Controller add " + name);
            onMessageAddDst.Invoke(name);
        }
    }

    void Echo(string src, string dst, string message, bool isRx)
    {
        Debug.Log("Echo " + src + ">>" + dst + ":"+ message);
    }

    public void Send(string src, string dst, string message, bool isRx)
    {
        onMessageSend.Invoke(src, dst, message, isRx);
        if (MessageSendActionRequest != null)
            MessageSendActionRequest(src, dst, message);
    }

    public void Receive(string src, string dst, string message, bool isRx)
    {
        onMessageSend.Invoke(src, dst, message, isRx);
    }
}
