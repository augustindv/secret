using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMarkerTuto : MonoBehaviour
{
    public GameObject decisionMarker;
    public bool isDecisionRunning;
    public string Target { get; set; }
    public bool TargetIsBank
    {
        get
        {
            return targetIsBank;
        }
        set
        {
            targetIsBank = value;
        }
    }

    public bool targetIsBank;
    private Text targetDbg;

    void Start()
    {
        decisionMarker = gameObject;
    }

    public void StartDecision()
    {
        decisionMarker.SetActive(true);
        isDecisionRunning = true;
    }

    public void StopDecision()
    {
        decisionMarker.SetActive(false);
        isDecisionRunning = false;
    }

    void FixedUpdate()
    {
        if (isDecisionRunning)
        {
            if (Input.GetMouseButton(0) == true)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = worldPoint;
            }
        }
    }

    void Update()
    {
        //Debug.Log("target " + Target);
        /*if (!targetDbg)
        {
            if (GameObject.Find("TargetDbg"))
                targetDbg = GameObject.Find("TargetDbg").GetComponent<Text>();
        }
        else
        {
            targetDbg.text = TargetIsBank ? "bank" : ((Target == null || Target =="none" ) ? "rien" : Target );
        }*/

    }
}
