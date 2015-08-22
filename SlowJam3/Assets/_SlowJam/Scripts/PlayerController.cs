using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinderBox;
public class PlayerController : MonoBehaviour
{
	float moveSpeed = 20.0f;
	Players myPlayer;
	private const float eggPenalty = 0.6f;
	private PlayerState myState = PlayerState.NOT_IN_PLAY;
	public static IList<PlayerController> players = new List<PlayerController>();
	private IList<StateChangeListener> stateChangeListeners = new List<StateChangeListener>();
    private float catchExpireTime = float.MinValue;
	public Vector3 delta = Vector3.zero;
	public bool isInWater;
	public bool isInTar;
	public float tarSlow = 0.5f;
	private const float staminaDecay = 0.1f;
	private const float staminaRegen = 0.1f;
	private const float staminaThrow = 0.3f;
	[Range(0,1)]
	public float stamina = 1.0f;
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
	private Animator charAnimator;
    public static Vector3 playerCenter
    {
        get {
            Vector3 pos = Vector3.zero;

			if(playerCount == 0) //avoid NaN errors before players have ready'd up
				return pos;

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
			if (isInWater && myState == PlayerState.HOLDING)
				return 0;
			float speed = 1.0f;
			if (isInTar)
				speed *= tarSlow;
			
			if (myState == PlayerState.HOLDING)
				speed*=eggPenalty;
			
			
			return moveSpeed*Time.deltaTime*speed;
		}

    }
	public PlayerState state {
		set {
			foreach(StateChangeListener scl in stateChangeListeners) {
				scl.playerStateChanged(myNumber, myState, value);
			}
			myState = value;
		}
		get {
			return myState;
		}
	}
	public void addStateChangeListener(StateChangeListener scl) {
		stateChangeListeners.Add(scl);
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
        
		charAnimator = GetComponentInChildren<Animator> ();
		eggNode = GetComponentInChildren<PlayerEggNode> ();
	}
    void Start()
    {
		SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer filter in meshes) {
			Mesh mesh = filter.sharedMesh;
			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			Vector2 pos = new Vector2 (0.5f, 0.5f);
			switch (myNumber) {
			case (0):
				pos = new Vector2 (0.2f, 0.8f);
				break;
			case (1):
				pos = new Vector2 (0.8f, 0.8f);
				break;
			case (2):
				pos = new Vector2 (0.8f, 0.2f);
				break;
			case (3):
				pos = new Vector2 (0.2f, 0.2f);
				break;
			}
			for (int i = 0; i < uvs.Length; i++) {
				uvs [i] = pos;
			}
			mesh.uv = uvs;
		}
    }
    public void PrepForGame()
    {
		state = PlayerState.NOT_HOLDING;
        players.Add(this);
        //TODO center this properly when can math.
        transform.position = new Vector3(-5+(1.0f/(PlayerManager.playerCount))*(playerCount)*10, 0.5f, 10);
    }
    public void UnloadPostGame()
    {
		state = PlayerState.NOT_IN_PLAY;
        players.Remove(this);
        PlayerManager.RemovedPlayerFromActive(myNumber);
        if (playerCount == 0 && _root.state == GameState.PLAYING)
        {
			_root.state = GameState.LOSE;
        }
        transform.position = new Vector3(0, 0, -9000);
    }
	void Update () {
        if (state == PlayerState.NOT_IN_PLAY) return;
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
            delta += moveDir * stepLength;
            //transform.position += moveDir * stepLength;
            Quaternion faceDir = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, faceDir, Time.deltaTime*10);
        }
        transform.position += delta * Time.deltaTime;
        delta -= delta * Time.deltaTime*5f;

		if (delta.magnitude < 0.8f) {
			charAnimator.SetFloat ("Speed", 0.8f);
			charAnimator.SetBool("IsMoving", false);
		}
		else {
			charAnimator.SetFloat ("Speed", (delta.magnitude * 0.3333333f));
			charAnimator.SetBool("IsMoving", true);
		}
		charAnimator.SetBool("HasEgg", state == PlayerState.HOLDING);
	}
	void StaminaUpdate()
	{
		if (state == PlayerState.HOLDING)
		{
			stamina -= staminaDecay * Time.deltaTime;
		}
		else if (stamina < 1)
		{
			stamina += staminaRegen * Time.deltaTime;
		}
		stamina = Mathf.Clamp01(stamina);
	}
	Vector3 TinderBoxMove() {
		Vector3 moveDir = Vector3.zero;
		if (TinderBoxAPI.ControlState(myNumber, TinderBox.Controls.Up))
		{
			moveDir += Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myNumber, TinderBox.Controls.Down))
		{
			moveDir -= Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myNumber, TinderBox.Controls.Left))
		{
			moveDir -= Vector3.right;
		}
		if (TinderBoxAPI.ControlState(myNumber, TinderBox.Controls.Right))
		{
			moveDir += Vector3.right;
		}
		Debug.DrawRay(transform.position, moveDir*2,Color.red);
		RaycastHit hit;
		if (Physics.Raycast(new Ray(transform.position, delta), out hit, stepLength))
		{
			if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
			{
				delta = Vector3.zero;
				moveDir = Vector3.zero;
			}
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
			QTE.PlayerEnter(myNumber);
			Debug.Log("QTE Trigger entered");
		}
		TarLogic tar = other.gameObject.GetComponent<TarLogic>();
		if(tar!=null){
			isInTar = true;
		}
		WaterLogic water = other.gameObject.GetComponent<WaterLogic>();
		if (water != null)
		{
			isInWater = true;
		}
		TailLogic tail = other.gameObject.GetComponent<TailLogic>();
		
		if (tail != null)
		{
			delta.z -= 50;
		}
		
	}

	void OnTriggerExit(Collider other)
	{
		QTEScript QTE = other.gameObject.GetComponent<QTEScript>();
		if (QTE != null)
		{
			QTE.PlayerExit(myNumber);
			Debug.Log("QTE Trigger exited");
		}
		TarLogic tar = other.gameObject.GetComponent<TarLogic>();
		if (tar != null)
		{
			isInTar = false;
		}
		WaterLogic water = other.gameObject.GetComponent<WaterLogic>();
		if (water != null)
		{
			isInWater = true;
		}
	}
    void NewButtons()
    {
        if (state == PlayerState.NOT_HOLDING && TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button5))
        {
            if (Time.time < catchExpireTime)
            {
                if (EggLogic.main.state != EggLogic._state.Held)
                {
                    EggLogic.main.PickUp(this);
                }
            }
        }
        if (state != PlayerState.HOLDING) return;
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button1))
        {
            int target = LookUp.PlayerLogicPosition(0);
			if (PlayerManager.registerdPlayers[target].state != PlayerState.NOT_IN_PLAY && target != myNumber) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
				stamina -= staminaThrow;
            }

        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button2))
        {
            int target = LookUp.PlayerLogicPosition(1);
			if (PlayerManager.registerdPlayers[target].state != PlayerState.NOT_IN_PLAY && target != myNumber) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
				stamina -= staminaThrow;
            }
        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button3))
        {
            int target = LookUp.PlayerLogicPosition(2);
			if (PlayerManager.registerdPlayers[target].state != PlayerState.NOT_IN_PLAY && target != myNumber) { 
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
				stamina -= staminaThrow;
            }
        }
        if (TinderBoxAPI.ControlDown(myNumber, TinderBox.Controls.Button4))
        {
            int target = LookUp.PlayerLogicPosition(3);
			if (PlayerManager.registerdPlayers[target].state != PlayerState.NOT_IN_PLAY && target != myNumber){
                Debug.Log(LookUp.PlayerColorName(myNumber) + " trew the ball to " + LookUp.PlayerColorName(target));
                Vector3 pos = PlayerManager.registerdPlayers[target].transform.position;
                EggLogic.main.ThrowToPlayer(pos);
				stamina -= staminaThrow;
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
		Debug.Log("Should be throwing the egg...");
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
