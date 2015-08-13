using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    float moveSpeed = 4.0f;
    public enum _state { Empty, Hold, QTE };
    public _state state = _state.Empty;
    public static PlayerController[] players = new PlayerController[4];
    public static int playerCount = 0;
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
                return moveSpeed * Time.deltaTime * 0.6f;
        }

    }
	string upKey="w";
	string downKey="s";
	string leftKey="a";
	string rightKey="d";
    void Awake()
    {
        if (playerCount >= 4)
        {
            Debug.LogError(gameObject.name + " Is trying to register as player (max players reached)");
        }
        
        players[playerCount] = this;
        RegisterKeys();
        playerCount++;
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            EggLogic.main.Throw();
        }
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
                break;
            case (1):
                upKey       = "up";
                downKey     = "down";
                leftKey     = "left";
                rightKey    = "right";
                break;
            case(2):
                upKey       = "i";
                downKey     = "k";
                leftKey     = "j";
                rightKey    = "l";
                break;
            
        }
    }
    void OnTriggerEnter(Collider other)
    {
        EggLogic egg = other.gameObject.GetComponent<EggLogic>();
        if (egg != null) egg.PickUp(this);
        //Debug.Log("EGG GET!");
    }
}
