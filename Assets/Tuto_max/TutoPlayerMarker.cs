using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutoPlayerMarker : NetworkBehaviour {
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
			if (isServer)
				targetIsBank = value;
		}
	}
	[SyncVar]
	public bool targetIsBank;


	void Start ()
	{
		decisionMarker = transform.gameObject;
		decisionMarker.SetActive(true);
		isDecisionRunning = true;
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
		if( isDecisionRunning)
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
	}
}