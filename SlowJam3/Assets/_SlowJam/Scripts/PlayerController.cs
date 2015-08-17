using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinderBox;
public class PlayerController : MonoBehaviour
{
    float moveSpeed = 4.0f;
	Players myPlayer;
	private const float eggPenalty = 0.6f;
    public enum _state { Empty, Hold, QTE ,NotInPlay};
    public _state state = _state.NotInPlay;
    public static IList<PlayerController> players = new List<PlayerController>();
    private float catchExpireTime = float.MinValue;
    private PlayerEggNode myEggNode;
    public PlayerEggNode eggNode
    {
        get { return myEggNode; }

        set { myEggNode = value; }
    }
	public static int playerCount
    {
        get { return players.Count; }
    }
    public int myNumber = -1;
    public static Vector3 playerCenter
    {
        get {
            Vector3 pos = Vector3.zero;
            for (int x = 0; x < playerCount; x++)
            {
                pos += players[x].transform.position;
            }
            return pos/playerCount;
        }
    }
    public float stepLength
    {
        get {
            if (state == _state.Empty)
                return moveSpeed * Time.deltaTime;
            else
                return moveSpeed * Time.deltaTime * eggPenalty;
        }

    }
	string upKey;
	string downKey;
	string leftKey;
	string rightKey;
	string button1;
	string button2;
	string button3;
	string button4;
	string button5;
    void Awake()
    {
		gameObject.layer = LayerMask.NameToLayer ("Player"); 	

        myNumber = PlayerManager.InitPlayerRegistration(this);
        if (myNumber == -1)
        {
            Destroy(gameObject);
            return;
        }
        if (playerCount >= 4)
        {
            Debug.LogError(gameObject.name + " Is trying to register as player (max players reached)");
        }
		//SetTinderBoxInputs (myNumber);

        //this.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Solid" + LookUp.PlayerColorName(myNumber)) as Material;
        
    }
    void Start()
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        Vector2 pos = new Vector2(0.5f, 0.5f);
        switch (myNumber)
        {
            case (0):
                pos = new Vector2(0.2f, 0.8f);
                break;
            case (1):
                pos = new Vector2(0.8f, 0.8f);
                break;
            case (2):
                pos = new Vector2(0.8f, 0.2f);
                break;
            case (3):
                pos = new Vector2(0.2f, 0.2f);
                break;
        }
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = pos;
        }
        mesh.uv = uvs;
    }
    public void PrepForGame()
    {
        state = _state.Empty;
        players.Add(this);
        //TODO center this properly when can math.
        transform.position = new Vector3(-5+(1.0f/(PlayerManager.playerCount))*(playerCount)*10, 0.5f, 10);
    }
    public void UnloadPostGame()
    {
        state = _state.NotInPlay;
        players.Remove(this);
        PlayerManager.RemovedPlayerFromActive(myNumber);
        if (playerCount == 0 && _root.state == _root._state.Playing)
        {
            _root.state = _root._state.Lose;
        }
        transform.position = new Vector3(0, 0, -9000);
    }
	void Update () {
        if (state == _state.NotInPlay) return;
        /*if (Input.GetKeyDown("space"))
        {
            EggLogic.main.Throw();
        }*/
        Debug.DrawLine(transform.position, playerCenter);
		//DetectButtons();
		//Vector3 moveDir = Move();
		NewButtons();
        Vector3 moveDir = TinderBoxMove();
        if (moveDir.magnitude != 0)
        {
            moveDir.Normalize();
            transform.position += moveDir * stepLength;
            Quaternion faceDir = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, faceDir, Time.deltaTime*10);
        }
	}
    Vector3 Move()
    {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(upKey))
        {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey(downKey))
        {
            moveDir -= Vector3.forward;
        }
        if (Input.GetKey(leftKey))
        {
            moveDir -= Vector3.right;
        }
        if (Input.GetKey(rightKey))
        {
            moveDir += Vector3.right;
        }
        Debug.DrawRay(transform.position, moveDir*2,Color.red);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, moveDir), out hit, stepLength))
        {
			if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            	moveDir = Vector3.zero;
        }
        return moveDir;
    }
	Vector3 TinderBoxMove() {
		Vector3 moveDir = Vector3.zero;
		if (TinderBoxAPI.ControlState(myPlayer, TinderBox.Controls.Up))
		{
			moveDir += Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myPlayer, TinderBox.Controls.Down))
		{
			moveDir -= Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myPlayer, TinderBox.Controls.Left))
		{
			moveDir -= Vector3.right;
		}
		if (TinderBoxAPI.ControlState(myPlayer, TinderBox.Controls.Right))
		{
			moveDir += Vector3.right;
		}
		Debug.DrawRay(transform.position, moveDir*2,Color.red);
		RaycastHit hit;
		if (Physics.Raycast(new Ray(transform.position, moveDir), out hit, stepLength))
		{
			if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
				moveDir = Vector3.zero;
		}
		return moveDir;
	}
	void RegisterKeys(int currentPlayerCount)
    {
		switch (currentPlayerCount)
        {
            case (0):
                upKey       = "w";
                downKey     = "s";
                leftKey     = "a";
                rightKey    = "d";
				button1		= "z";
				button2		= "x";
				button3		= "c";
				button4		= "v";
				button5		= "b";
                break;
            case (1):
                upKey       = "up";
                downKey     = "down";
                leftKey     = "left";
                rightKey    = "right";
				button1		= "n";
				button2		= "m";
				button3		= ",";
				button4		= ".";
				button5		= "/";
                break;
            case(2):
                upKey       = "u";
                downKey     = "j";
                leftKey     = "h";
                rightKey    = "k";
				button1		= "i";
				button2		= "o";
				button3		= "p";
				button4		= "[";
				button5		= "]";
                break;
            case (3):
                upKey       = "2";
                downKey     = "3";
                leftKey     = "1";
                rightKey    = "4";
				button1		= "5";
				button2		= "6";
				button3		= "7";
				button4		= "8";
				button5		= "9";
                break;
        }

		Debug.Log ("Player " + myPlayer + ": " + upKey + " " + downKey + " " + leftKey + " " + rightKey + " " + button1 + " " + button2 + " " + button3 + " " + button4 + " " + button5 + "!!");
    }

    void OnTriggerEnter(Collider other)
    {
        /*Debug.Log("Player hit trigger");
        EggLogic egg = other.gameObject.GetComponent<EggLogic>();
        if (egg != null) 
        { 
            egg.PickUp(this);
            catchExpireTime = Time.time + 0.3f;
        }*/

		QTEScript QTE = other.gameObject.GetComponent<QTEScript>();
		if (QTE != null)
		{
			QTE.PlayerEnter(myPlayer);
			Debug.Log("QTE Trigger entered");
		}
    }

	void OnTriggerExit(Collider other)
	{
		QTEScript QTE = other.gameObject.GetComponent<QTEScript>();
		if (QTE != null)
		{
			QTE.PlayerExit(myPlayer);
			Debug.Log("QTE Trigger exited");
		}
	}
    void NewButtons()
    {
        if (state == _state.Empty && TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button5))
        {
            if (Time.time < catchExpireTime)
            {
                if (EggLogic.main.state != EggLogic._state.Held)
                {
                    EggLogic.main.PickUp(this);
                }
            }
        }
        if (state != _state.Hold) return;
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button1))
        {
            int target = LookUp.PlayerLogicPosition(0);
            if (PlayerManager.registerdPlayers[target].state != PlayerController._state.NotInPlay) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
            }

        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button2))
        {
            int target = LookUp.PlayerLogicPosition(1);
            if (PlayerManager.registerdPlayers[target].state != PlayerController._state.NotInPlay) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
            }
        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button3))
        {
            int target = LookUp.PlayerLogicPosition(2);
            if (PlayerManager.registerdPlayers[target].state != PlayerController._state.NotInPlay) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
            }
        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button4))
        {
            int target = LookUp.PlayerLogicPosition(3);
            if (PlayerManager.registerdPlayers[target].state != PlayerController._state.NotInPlay){
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
            }
        }
    }
    void DetectButtons()
	{	
		Debug.Log ("Player " + myPlayer + ": " + upKey + " " + downKey + " " + leftKey + " " + rightKey + " " + button1 + " " + button2 + " " + button3 + " " + button4 + " " + button5 + "!!");
        // Blue Button on the TinderBox
        if (Input.GetKeyDown(button1))
        {
			if(EggLogic.main.heldBy == this) return;
			DoThrow(0);
        }
        // Red Button on the TinderBox
        if (Input.GetKeyDown(button2))
        {
			if(EggLogic.main.heldBy == this) return;
			DoThrow (1);
        }
        // Yellow Button on the TinderBox
        if (Input.GetKeyDown(button3))
        {
			if(EggLogic.main.heldBy == this) return;
			DoThrow (2);
        }
        // Green Button on the TinderBox
        if (Input.GetKeyDown(button4))
        {
			if(EggLogic.main.heldBy == this) return;
			DoThrow (3);
        }
        // White Button on the TinderBox
        if (Input.GetKeyDown(button5))
        {
            EggLogic.main.Throw();
        }
        //}
    }
	private void DoThrow (int playerIndex) {
		Debug.Log("Should be throwing the egg..");
		//int temp = LookUp.PlayerCabinetPosition(playerIndex);
		Vector3 position = PlayerManager.registerdPlayers[LookUp.PlayerCabinetPosition(playerIndex)].transform.position;
		if (myNumber == playerIndex)
		{
			Debug.Log ("Throwing To Self");
			EggLogic.main.Throw();
		}
		EggLogic.main.ThrowToPlayer(position);
	}
	// Set the myPlayer variable to the 
	// position on the TinderBox so we
	// can coordinate input detection.
	public void SetTinderBoxInputs(int playerNumber) {
		switch (playerNumber) {
			case 0: 
				myPlayer = Players.Player1;
				break;
			case 1:
				myPlayer = Players.Player2;
				break;
			case 2:
				myPlayer = Players.Player3;
				break;
			case 3:
				myPlayer = Players.Player4;
				break;
			default:
			Debug.Log("Error in SetTinderBoxInputs.  Too many players, or invalid value.");
				break;
		}
		RegisterKeys (playerNumber);
	}
}
