using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageAdd : MonoBehaviour {
    public GameObject rxPrefab;
    public GameObject txPrefab;

    public void Add(string src, string dst, string message, bool isRx)
    {
        if(isRx)
        {
            Debug.Log("Add Rx" + src + ">>" + dst + ":" + message);

            // TODO pas très beau ... il faudrait ajouter un script sur le prefab
            GameObject go = GameObject.Instantiate(rxPrefab, transform);
            Sprite sprite = PlayerDatabase.instance.GetSprite(src);
            go.transform.FindChild("Image").gameObject.GetComponent<Image>().sprite = sprite;
            // TODO why do we need to enforce this ?
            // Force scale to 1 the prefab starts with 
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.FindChild("Src").gameObject.GetComponent<Text>().text = src;
            go.transform.FindChild("Message").gameObject.GetComponent<Text>().text = message;
        }
        else
        {
            Debug.Log("Add Tx" + src + ">>" + dst + ":" + message);

            // TODO pas très beau ... il faut drait ajouter un script sur le prefab
            GameObject go = GameObject.Instantiate(txPrefab, transform);
            // TODO why do we need to enforce this ?
            Sprite sprite = PlayerDatabase.instance.GetSprite(dst);
            go.transform.FindChild("Image").gameObject.GetComponent<Image>().sprite = sprite;
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.FindChild("Message").gameObject.GetComponent<Text>().text = message;
            go.transform.FindChild("Dst").gameObject.GetComponent<Text>().text = dst.ToString();
        }

        // TODO update pos
        Debug.Log(gameObject.GetComponent<RectTransform>().rect.height);
        Debug.Log(gameObject.GetComponent<RectTransform>().position);
        Vector3 pos = gameObject.GetComponent<RectTransform>().position;
        pos.y = gameObject.GetComponent<RectTransform>().rect.height + 1000;
        gameObject.GetComponent<RectTransform>().position = pos;
    }
}
