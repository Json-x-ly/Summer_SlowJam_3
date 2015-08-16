using UnityEngine;
using System.Collections;
using System;

public  class QTEScript : MonoBehaviour 
{
	private bool isActive;
	private float _progress;
	public float progress{ 
		get
		{
			return _progress;
		} 
		set
		{
			addProgress(value);
		} 
	};

	public enum QTE
	{
		test, mash, alternate
	}

	public QTE QTEType;

	void update() 
	{
		if (!isActive)
			return;
	}

	public void triggerEvent(Action OnComplete)
	{
		switch (Type) {
		default:
			break;
		case QTE.test:
			isActive = true;
			break;
		}
	}
	private void addProgress(float amt)
	{
		if (amt >= 1.0f)
			amt = 1.0f;
		if (amt <= 0.0f)
			amt = 0.0f;

		_progress += amt;
	}
}