using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeProgressBar : MonoBehaviour {

    public GameObject progress;

    public void ChangeProgressBarWidth(float width)
    {
        progress.GetComponent<RectTransform>().sizeDelta = new Vector2(width, progress.GetComponent<RectTransform>().sizeDelta.y);
    }
}
