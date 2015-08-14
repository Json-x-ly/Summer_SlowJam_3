using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinderBox;
public class PlayerController : MonoBehaviour
{
    float moveSpeed = 4.0f;
	private const float eggPenalty = 0.6f;
    public enum _state { Empty, Hold, QTE };
    public _state state = _state.Empty;
    public static IList<PlayerController> players = new List<PlayerController>();
    public static int playerCount
    {
        get { return players.Count; }
    }
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
        if (playerCount >= 4)
        {
            Debug.LogError(gameObject.name + " Is trying to register as player (max players reached)");
            Destroy(gameObject);
            return;
        }

        RegisterKeys();
        players.Add(this);
    }
	// Update is called once per frame
	void Update () {
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
    void RegisterKeys()
    {
        switch (playerCount)
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
}
