using UnityEngine;
using System.Collections;

public class RestartLogic : MonoBehaviour {
	public int last = 0;
	public int selection = 0;
	private const float quitX = 5;
	private const float restartX = -5;
	private Vector3 targetPos;
	public int player=-1;

	void Awake () {
		targetPos = transform.position;
	}

	void Update () {
		if (last != selection) { //I did this part wrong but it works well enough, it's just for changing the value in the inspector
			if(selection == 1)
				targetPos.x = restartX;
			else if(selection == 2)
				targetPos.x = quitX;
		}
		last = selection;

		if(TinderBox.TinderBoxAPI.ControlDown(player,TinderBox.Controls.Left)) {
			selection = 1;
			last = 1;
			targetPos.x = restartX;
		} else if(TinderBox.TinderBoxAPI.ControlDown(player,TinderBox.Controls.Right)) {
			selection = 2;
			last = 2;
			targetPos.x = quitX;
		}
		transform.position=Vector3.Lerp(transform.position, targetPos, Time.deltaTime*10);
	}

	public void SetPlayerNumber(int player) {
		this.player = player;
		MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
		mr.material = Resources.Load("Solid"+ LookUp.PlayerColorName(player))as Material;
	}

	public void RemoveReadyCard() {
		Destroy(gameObject);
	}
}
