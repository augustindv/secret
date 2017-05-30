using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageAdd : MonoBehaviour {
    public GameObject rxPrefab;
    public GameObject txPrefab;
    public GameObject messageNew;
    public int MessageNewCount { get; set; }
    public Text messageNewText;

    public void ChangeTab(int newScreen)
    {
        if (newScreen == 1)
            MessageNewCount = 0;
    }

    public void scrollValue(Vector2 scrollpos)
    {
        if(scrollpos.y <= 0)
        {
            MessageNewCount = 0;
            messageNew.SetActive(false);
        }
    }

    private Vector2 GetSize(string text, Text textBox)
    {
        TextGenerator textGen = new TextGenerator();
        Vector2 size = textBox.rectTransform.rect.size;
        size.x *= Screen.width / 1080;
        size.y *= Screen.height / 1920;
        TextGenerationSettings generationSettings = textBox.GetGenerationSettings(size);
        generationSettings.generationExtents = new Vector2(550, 300);
        float width = textGen.GetPreferredWidth(text, generationSettings);
        float height = textGen.GetPreferredHeight(text, generationSettings);

        Debug.LogWarning("GetSize " + width + " x " + height);
        return new Vector2(width, height);
    }

    public void Add(string src, string dst, string message, bool isRx)
    {
        if(isRx)
        {
            Debug.Log("Add Rx" + src + ">>" + dst + ":" + message);

            // TODO pas très beau ... il faudrait ajouter un script sur le prefab
            GameObject go = GameObject.Instantiate(rxPrefab, transform);
            Sprite sprite = PlayerDatabase.instance.GetSprite(src);
            Sprite chest = PlayerDatabase.instance.GetChestSprite(src);
            go.transform.FindChild("Image").gameObject.GetComponent<Image>().sprite = sprite;
            go.transform.FindChild("Chest").gameObject.GetComponent<Image>().sprite = chest;
            // TODO why do we need to enforce this ?
            // Force scale to 1 the prefab starts with 
            go.transform.localScale = new Vector3(1, 1, 1);

            go.transform.FindChild("Src").gameObject.GetComponent<Text>().text = src;
            go.transform.FindChild("Message").gameObject.GetComponent<Text>().text = message;
            Vector2 sizeDelta = GetSize(message, go.transform.FindChild("Message").gameObject.GetComponent<Text>());
            RectTransform rectTransform = go.transform.FindChild("Background").gameObject.GetComponent<RectTransform>();
            sizeDelta.x *= 1080 / Screen.width;
            sizeDelta.y *= 1920 / Screen.height;
            sizeDelta += new Vector2(60, 60);
#if UNITY_IOS
            rectTransform.sizeDelta = sizeDelta;
#endif
            go.transform.FindChild("Background").gameObject.GetComponent<Image>().color = PlayerDatabase.instance.GetDarkerColor(src);
            messageNew.SetActive(true);
            messageNewText.text = (++MessageNewCount).ToString();
        }
        else
        {
            Debug.Log("Add Tx" + src + ">>" + dst + ":" + message);

            // TODO pas très beau ... il faut drait ajouter un script sur le prefab
            GameObject go = GameObject.Instantiate(txPrefab, transform);
            // TODO why do we need to enforce this ?
            Sprite sprite = PlayerDatabase.instance.GetSprite(dst);
            Sprite chest = PlayerDatabase.instance.GetChestSprite(dst);
            go.transform.FindChild("Image").gameObject.GetComponent<Image>().sprite = sprite;
            go.transform.FindChild("Chest").gameObject.GetComponent<Image>().sprite = chest;
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.FindChild("Message").gameObject.GetComponent<Text>().text = message;
            go.transform.FindChild("Dst").gameObject.GetComponent<Text>().text = dst.ToString();
            Vector2 sizeDelta = GetSize(message, go.transform.FindChild("Message").gameObject.GetComponent<Text>());
            RectTransform rectTransform = go.transform.FindChild("Background").gameObject.GetComponent<RectTransform>();
            sizeDelta.x *= 1080 / Screen.width;
            sizeDelta.y *= 1920 / Screen.height;
            sizeDelta += new Vector2(60, 60);
            Debug.LogWarning("SizeDelta " + sizeDelta);
#if UNITY_IOS
            rectTransform.sizeDelta = sizeDelta;
#endif
            //go.transform.FindChild("Background").gameObject.GetComponent<Image>().color = PlayerDatabase.instance.GetColor(src);
        }

        // TODO update pos
        //Debug.Log(gameObject.GetComponent<RectTransform>().rect.height);
        //Debug.Log(gameObject.GetComponent<RectTransform>().position);
        /* Vector3 pos = gameObject.GetComponent<RectTransform>().position;
        pos.y = gameObject.GetComponent<RectTransform>().rect.height + 1000;
        gameObject.GetComponent<RectTransform>().position = pos; */
    }
}
