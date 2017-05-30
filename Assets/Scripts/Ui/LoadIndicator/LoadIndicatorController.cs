using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadIndicatorController : MonoBehaviour {

    public Text loadText;
    public GameObject panel;

    public static LoadIndicatorController loadIndicatorController;
    public static LoadIndicatorController instance
    {
        get
        {
            if (!loadIndicatorController)
            {
                loadIndicatorController = FindObjectOfType(typeof(LoadIndicatorController)) as LoadIndicatorController;

                if (!loadIndicatorController)
                {
                    Debug.LogError("There needs to be one active ProfileController script on a GameObject in your scene.");
                }
            }

            return loadIndicatorController;
        }
    }

    public void Activate (bool activate)
    {
        panel.SetActive(activate);
    }

    public void setLoad(int load)
    {
        loadText.text = load.ToString() + "%";
    }
}
