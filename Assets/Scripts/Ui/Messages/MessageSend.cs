﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSend : MonoBehaviour {
    public string src = "moa";
    public Dropdown dst;
    public InputField message;
    //public MessageUnityEvent onMessageSend;

    // Update is called once per frame
    public void Send()
    {
        if(message.text != "")
        {
            MessageController.instance.Send(MessageController.instance.PlayerName, dst.options[dst.value].text, message.text, false);
            message.text = "";
        }
    }

    public void Start()
    {
        //message.onEndEdit.AddListener(delegate { Send(); });
    }
}
