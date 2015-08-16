using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TinderBox;
public  class QTEScript : MonoBehaviour 
{
	private bool isActive;
	private float _progress;
	public float progress
	{ 
		get
		{
			return _progress;
		}
		set
		{
			AddProgress(value);
		} 
	}

	private Action onComplete;

	public enum QTE
	{
		test, mash, alternate
	}

	public QTE QTEType;
	private List<Players> playerList;


	void Start()
	{
		playerList = new List<Players>();
	}

	void Update()
	{
		if (!isActive)
			return;


	}

	public void PlayerEnter (Players playerID) 
	{
		if (playerList.Count > 0) {
			foreach (Players ID in playerList) {
				if (ID.CompareTo (playerID) == 0)
					continue;
				return;
			}
		}
		playerList.Add (playerID);
		isActive = true;
	}

	public void PlayerExit (Players playerID)
	{
		playerList.Remove(playerID);
		if (playerList.Count == 0)
			isActive = false;
	}

	private void AddProgress(float amt)
	{
		if (amt >= 1.0f)
			amt = 1.0f;
		if (amt <= 0.0f)
			amt = 0.0f;

		_progress += amt;
	}
}