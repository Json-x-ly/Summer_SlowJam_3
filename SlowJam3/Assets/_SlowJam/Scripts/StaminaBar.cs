using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaBar : MonoBehaviour {
	
	private long stamina = 100;
	public GameObject staminaBar;

	void Start()
	{
		staminaBar = Instantiate (staminaBar);
		//staminaBar.transform.parent = transform;
		staminaBar.transform.position = Vector3.zero;
	}

	void Update()
	{
		staminaBar.transform.position = transform.position;
		Debug.Log (transform.position);
		Slider stamScript = staminaBar.GetComponentInChildren<Slider> ();
		stamScript.value = stamina;
	}
}