using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DebugController : MonoBehaviour {
    public GameObject DebugView;
    public GameObject logItem;
    public Transform content;
    public int maxLogs = 10;
    private List<GameObject> logItems = new List<GameObject>();
    public Button exitButton;
    public bool hide = true;

	void Start ()
    {
        Application.logMessageReceived += AddLog;
        exitButton.onClick.AddListener(delegate {
            DebugView.SetActive(false);
            ShowCaptainsMess(false);
        });
        ShowCaptainsMess(false);
    }

    private void ShowCaptainsMess(bool doShow)
    {
        GameObject cm = GameObject.Find("CaptainsMessNetworkManager");
        if (cm) cm.GetComponent<CaptainsMessDebugGUI>().enabled = doShow;
    }

    private void AddLogItem(string text, string prefix, LogType type)
    {
        GameObject go;
        if (logItems.Count > maxLogs) // destroy does not work well => recycle is better
        {
            go = logItems[0];
            logItems.RemoveAt(0);
            go.transform.parent = null; // unparent to move at the bottom of the view later
        }
        else
        {
            go = GameObject.Instantiate(logItem);
        }

        if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
        {
            int count = 1 + Regex.Matches(text, @"~").Count;
            go.transform.FindChild("Text").GetComponent<Text>().text = "<color=#FF0000>" + prefix + text + "</color>";
            go.transform.FindChild("Text").GetComponent<Text>().fontSize = 40;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(go.GetComponent<RectTransform>().sizeDelta.x, 20 + count * 50);
        }
        else if (type == LogType.Warning)
            go.transform.FindChild("Text").GetComponent<Text>().text = "<color=#FFFF00>" + prefix + text + "</color>";
        else
            go.transform.FindChild("Text").GetComponent<Text>().text = "<color=#00FF00>" + prefix +  text + "</color>";
        logItems.Add(go);
        go.transform.parent = content;
    }
    private void AddLog(string condition, string stackTrace, LogType type)
    {
        AddLogItem(condition, "", type);

        if(type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
        {
            string[] texts = stackTrace.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            if(!hide)
                DebugView.SetActive(true);

            foreach (string text in texts)
            {
                string rest = System.String.Copy(text);
                string display = "";
                while(rest.Length > 45)
                {
                    display = display + rest.Substring(0, 45) + "~\n";
                    rest = rest.Substring(45);
                }
                display = display + rest;
                AddLogItem(display, Time.fixedTime +  ">", type);
            }
        }
    }

    void Update()
    {
        if (!DebugView.activeSelf &&  Input.acceleration.y > 0.8f && !hide)
        {
            DebugView.SetActive(true);
            ShowCaptainsMess(true);
        }
            
    }
}
