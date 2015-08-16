using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	
	private long progress = 50;
	public GameObject progressBar;
	
	void Start()
	{
		progressBar = Instantiate (progressBar);
		progressBar.transform.parent = transform;
	}
	
	void Update()
	{
		Debug.Log (progressBar.transform.position);
		Slider progScript = progressBar.GetComponentInChildren<Slider> ();
		progScript.value = progress;
	}
}