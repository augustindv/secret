using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.UI;  [RequireComponent(typeof(Text))]
public class MessageEcho : MonoBehaviour
{
    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Echo(string src, string dst, string message, bool isRx)
    {
        text.text = src + " " + dst + " " + message + " rx:" + isRx;
    }
}