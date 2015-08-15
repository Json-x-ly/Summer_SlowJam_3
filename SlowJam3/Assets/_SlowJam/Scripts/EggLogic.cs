using UnityEngine;
using System.Collections;

public class EggLogic : MonoBehaviour {
    public static EggLogic main;
	public float gravity = 5.0f;
    public enum _state { Throwing,Falling, OnGround, Held };
    public _state state = _state.OnGround;
    public Vector3 delta;
    public PlayerController heldBy;
    void Awake()
    {
        main = this;
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case(_state.Held):
                transform.position = heldBy.transform.position + Vector3.up;
                break;
            case(_state.Throwing):
            case(_state.Falling):
				transform.position += delta * Time.deltaTime;
				delta.y -= gravity * Time.deltaTime;
				RaycastHit hitinfo;
				Debug.DrawRay(transform.position, delta * Time.deltaTime, Color.red);
				if(Physics.Raycast(new Ray(transform.position, delta * Time.deltaTime), out hitinfo)) {
					if(hitinfo.distance < 1.0f){
					/*if(hitinfo.collider.gameObject.layer == LayerMask.GetMask("Terrain")) {*/
						state = _state.OnGround;	
						transform.position = hitinfo.point; 
						Debug.Log(hitinfo.point);
						break;
					//}
					}
				}
				break;

                /*transform.position += delta * Time.deltaTime;
                delta.y -= 11f * Time.deltaTime;
                if (delta.y < 0) state = _state.Falling;
                if (transform.position.y < 0)
                {
                    Vector3 temp = transform.position;
                    temp.y = 0;
                    transform.position = temp;
                    state = _state.OnGround;
                }
                break;*/
        }
	}
    public void PickUp(PlayerController pc)
    {
        if (state == _state.OnGround || state == _state.Falling)
        {
            heldBy = pc;
            state = _state.Held;
            pc.state = PlayerController._state.Hold;
        }
    }
    public void Throw()
    {
        Debug.Log("I am being thrown!");
        if (state == _state.Held)
        {
            state = _state.Throwing;
            Vector3 pos = heldBy.transform.position;

            PlayerController nearest=heldBy;
            float nearestDist = float.MaxValue;
            foreach (PlayerController player in PlayerController.players)
            {
                if (player == heldBy) continue;
                float currentDist = Vector3.Distance(player.transform.position, heldBy.transform.position);
                if (nearestDist > currentDist)
                {
                    nearestDist=currentDist;
                    nearest=player;
                }
            }
            //ThrowStriaght();
            ThrowAt(nearest.transform.position);
            heldBy.state = PlayerController._state.Empty;
        }
    }
    private void ThrowStriaght()
    {
        delta = (heldBy.transform.forward + Vector3.up) * 5;
    }
    private void ThrowAt(Vector3 dest)
    {
        delta = Vector3.Normalize((dest - heldBy.transform.position) + Vector3.up) * 5;
    }
	// Detect Ground Hit
	// 
}
