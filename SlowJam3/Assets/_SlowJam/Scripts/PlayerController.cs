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
	private PlayerEggNode myEggNode;
	public PlayerEggNode eggNode {
		get{ return myEggNode;}

		set { myEggNode = value;}
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
		SetTinderBoxInputs (myNumber);
		myEggNode = this.GetComponentInChildren<PlayerEggNode> ();
    }
    public void PrepForGame()
    {
        state = _state.Empty;
        players.Add(this);
    }
    public void UnloadPostGame()
    {
        state = _state.NotInPlay;
        players.Remove(this);
    }
	void Update () {
        if (state == _state.NotInPlay) return;
        if (Input.GetKeyDown("space"))
        {
            EggLogic.main.Throw();
        }
		DetectButtons();
        Debug.DrawLine(transform.position, playerCenter);
        Vector3 moveDir = Move();
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
            moveDir = Vector3.zero;
        }
        return moveDir;
    }
	Vector3 TinderBoxMove() {
		Vector3 moveDir = Vector3.zero;
		if (TinderBoxAPI.ControlState(myPlayer, Controls.Up))
		{
			moveDir += Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myPlayer, Controls.Down))
		{
			moveDir -= Vector3.forward;
		}
		if (TinderBoxAPI.ControlState(myPlayer, Controls.Left))
		{
			moveDir -= Vector3.right;
		}
		if (TinderBoxAPI.ControlState(myPlayer, Controls.Right))
		{
			moveDir += Vector3.right;
		}
		//moveDir = Vector3.zero;
		Debug.DrawRay(transform.position, moveDir*2,Color.red);
		RaycastHit hit;
		if (Physics.Raycast(new Ray(transform.position, moveDir), out hit, stepLength))
		{
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
    }
    void OnTriggerEnter(Collider other)
    {
        EggLogic egg = other.gameObject.GetComponent<EggLogic>();
        if (egg != null) egg.PickUp(this);
        //Debug.Log("EGG GET!");
    }
	void DetectButtons() {
		// Blue Button on the TinderBox
		if(Input.GetKeyDown(button1)) {

		}
		// Red Button on the TinderBox
		if(Input.GetKeyDown(button2)) {

		}
		// Yellow Button on the TinderBox
		if(Input.GetKeyDown(button3)) {

		}
		// Green Button on the TinderBox
		if(Input.GetKeyDown(button4)) {

		}
		// White Button on the TinderBox
		if(Input.GetKeyDown(button5)) {
			EggLogic.main.Throw();
		}
	}
	// Set the myPlayer variable to the 
	// position on the TinderBox so we
	// can coordinate input detection.
	public void SetTinderBoxInputs(int currentPlayerCount) {
		switch (myNumber) {
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
		RegisterKeys (currentPlayerCount);
	}
}
