using UnityEngine;
using System.Collections;

public class GameEndManager : MonoBehaviour, StateChangeListener {
	private bool[] playersDead = { false, false, false, false };
	private RestartLogic[] restartLogics = new RestartLogic[4];
	private int selection = 0;
	public GameObject winThing;
	private bool endScreenActive = false;

	void Start () {
		foreach(PlayerController pc in PlayerManager.registerdPlayers) {
			pc.addStateChangeListener(this);
		}
		_root.addStateChangeListener(this);
		//showWinScreen ();
	}

	void Update () {
		if (endScreenActive) {
			bool restart = true;
			bool quit = false;
			for(int i = 0; i < 4; i++) {
				if(!PlayerManager.IsPlayerActive(i))
					continue;
				if(restartLogics[i].selection != 1)
					restart = false;
				if(restartLogics[i].selection == 2)
					quit = true;
			}
			if(restart) {
				_root.state = GameState.PLAYING;
				despawnCards();
			} else if(quit) {
				_root.state = GameState.READY;
				despawnCards();
			}
		}
	}

	public void playerStateChanged(int playerIndex, PlayerState oldState, PlayerState newState) {
		Debug.Log ("A player state change was received!");
		if(newState == PlayerState.DEAD)
			playersDead[playerIndex] = true;
		bool lost = true;
		for (int i = 0; i < 4; i++) {
			if(!PlayerManager.IsPlayerActive(i))
				continue;
			if(!playersDead[i]) {
				lost = false;
				break;
			}
		}
		if(lost)
			_root.state = GameState.LOSE;
	}

	public void gameStateChanged(GameState oldState, GameState newState) {
		Debug.Log ("A game state change was received!");

		if(newState == GameState.WIN)
			showWinScreen();
		if(newState == GameState.LOSE)
			showLoseScreen();
	}

	public void showLoseScreen() {

	}

	public void showWinScreen() {
		endScreenActive = true;
		GameObject doge = Instantiate(winThing);
		doge.layer = LayerMask.NameToLayer("UI");

		spawnCards();
	}
	
	private void spawnCards() {
		Vector3 rcStartPos = new Vector3(0f, 9f, 1f);
		Vector3 rcSize = new Vector3(3, 1, 1);
		const float rcStepLength = 1f + 1f / 3f;
		for (int x = 0; x < 4; x++) {
			if(!PlayerManager.IsPlayerActive(x))
				continue;
			int cabPos = LookUp.PlayerCabinetPosition(x);
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
			go.name = "RestartPlayer " + x;
			go.transform.position = rcStartPos + Vector3.up * cabPos * rcStepLength;
			go.transform.localScale = rcSize;
			go.layer = LayerMask.NameToLayer("UI");
			RestartLogic script = go.AddComponent<RestartLogic>();
			script.SetPlayerNumber(x);
			restartLogics[x] = script;
		}
	}

	private void despawnCards() {
		endScreenActive = false;
		foreach (RestartLogic rl in restartLogics) {
			Destroy(rl.gameObject);
		}
	}
}