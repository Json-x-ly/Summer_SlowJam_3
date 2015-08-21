using UnityEngine;
using System.Collections;

public class GameEndManager : MonoBehaviour, StateChangeListener {
	public GameObject winThing;
	public GameObject loseThing;

	private bool[] playersDead = { false, false, false, false };
	private RestartLogic[] restartLogics = new RestartLogic[4];
	private GUIStyle guiStyle = new GUIStyle();
	private GameObject doge;
	private GameObject star;
	private int selection = 0;
	private bool endScreenActive = false;
	private float selectionFinishTime = -1f;

	void Start () {
		foreach(PlayerController pc in PlayerManager.registerdPlayers) {
			pc.addStateChangeListener(this);
		}
		_root.addStateChangeListener(this);
		//showWinScreen ();
	}

	void Update () {
		if (endScreenActive) {
			if(selectionFinishTime != -1f && selectionFinishTime < Time.fixedTime) {
				if(selection == 2)
					_root.state = GameState.READY;
				else if(selection == 1)
					_root.state = GameState.READY;
				despawnCards();
			}
			bool restart = true;
			bool quit = false;
			bool nothing = true;
			for(int i = 0; i < 4; i++) {
				if(!PlayerManager.IsPlayerActive(i))
					continue;
				if(restartLogics[i].selection != 1)
					restart = false;
				if(restartLogics[i].selection == 2)
					quit = true;
			}
			if(restart) {
				selection = 1;
				if(selectionFinishTime == -1f)
					selectionFinishTime = Time.fixedTime + 10;
				//_root.state = GameState.PLAYING;
				//despawnCards();
			} else if(quit) {
				selection = 2;
				if(selectionFinishTime == -1f)
					selectionFinishTime = Time.fixedTime + 10;
				//_root.state = GameState.READY;
				//despawnCards();
			}
		}
	}

	void OnGUI() {
		if (endScreenActive) {
			guiStyle.alignment = TextAnchor.UpperCenter;
			guiStyle.fontSize = 42;
			string text = "Select an option";
			if(selectionFinishTime != -1f)
				text += " (" + (int) (selectionFinishTime - Time.fixedTime) + ")";
			GUI.Label (new Rect (Screen.width / 2, 10, 0, 0), text, guiStyle);
			guiStyle.fontSize = 28;

			if(selection == 1)
				guiStyle.normal.textColor = Color.green;
			else
				guiStyle.normal.textColor = Color.black;
			GUI.Label (new Rect (250, 20, 0, 0), "Restart", guiStyle);

			if(selection == 2)
				guiStyle.normal.textColor = Color.green;
			else
				guiStyle.normal.textColor = Color.black;
			GUI.Label (new Rect (Screen.width - 250, 20, 0, 0), "Quit", guiStyle);
			guiStyle.normal.textColor = Color.black;
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
		endScreenActive = true;
		star = Instantiate(loseThing);
		star.layer = LayerMask.NameToLayer("UI");
		
		spawnCards();
	}

	public void showWinScreen() {
		endScreenActive = true;
		doge = Instantiate(winThing);
		doge.layer = LayerMask.NameToLayer("UI");

		spawnCards();
	}
	
	private void spawnCards() {
		Vector3 rcStartPos = new Vector3(0f, 9f, 1f);
		Vector3 rcSize = new Vector3(3, 1, 1);
		const float rcStepLength = 1f + 1f / 3f;
		for (int x = 0; x < 4; x++) {
			//if(!PlayerManager.IsPlayerActive(x))
				//continue;
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

	public void despawnCards() {
		endScreenActive = false;
		selectionFinishTime = -1f;
		selection = 0;
		foreach (RestartLogic rl in restartLogics) {
			if(rl != null)
				Destroy(rl.gameObject);
		}
		if(doge != null)
			Destroy(doge);
		if(star != null)
			Destroy(star);
	}
}