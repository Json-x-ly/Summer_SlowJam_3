﻿using UnityEngine;
using System.Collections;

public class EggLogic : MonoBehaviour {
    public static EggLogic main;
	public float gravity = 5.0f;
    public enum _state { Throwing,Falling, OnGround, Held };
    public _state state = _state.OnGround;
    public Vector3 delta;
    public PlayerController heldBy;
	public GameObject shadowBigPrefab;
	//public GameObject shadowSmallPrefab;
	private GameObject shadowBigObject;
	//private GameObject shadowSmallObject;
	private const float MAX_SHADOW = 5.0f;
    void Awake()
    {
        main = this;
		shadowBigObject = (GameObject)Instantiate (shadowBigPrefab, Vector3.zero, Quaternion.identity);
		//shadowSmallObject = (GameObject)Instantiate (shadowSmallPrefab, Vector3.zero, Quaternion.identity);
		Destroy (shadowBigObject.GetComponent<BoxCollider> ());
		//Destroy (shadowSmallObject.GetComponent<BoxCollider> ());
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
				DrawFallingShadow();
				DetectGroundHit();
				/*transform.position += delta * Time.deltaTime;
				delta.y -= gravity * Time.deltaTime;
				RaycastHit fallingHitinfo;
				if(Physics.Raycast(new Ray(transform.position, Vector3.down), out fallingHitinfo)) {
					if(fallingHitinfo.distance < 0.1f){
						state = _state.OnGround;	
						transform.position = fallingHitinfo.point; 
						break;
					}
				}*/
				break;
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
	private void DrawFallingShadow() {
		RaycastHit hitinfo;
		Debug.DrawRay (transform.position, Vector3.down * 10.0f, Color.red);
		if (Physics.Raycast (new Ray (transform.position, Vector3.down), out hitinfo)) {
			Vector3 newVector = hitinfo.point;
			shadowBigObject.transform.position = newVector;// - new Vector3(0.0f, 1f, 0.0f);
			if(hitinfo.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				float scalar = (this.transform.position - hitinfo.point).magnitude / MAX_SHADOW;
				if(scalar > 1.0f) scalar = 1.0f;
				Debug.Log(scalar);
				shadowBigObject.transform.localScale = new Vector3(scalar, scalar, scalar);
				Debug.Log(shadowBigObject.transform.localScale);
			}
		}
	}
	private void DetectGroundHit() {
		DrawFallingShadow();
		transform.position += delta * Time.deltaTime;
		delta.y -= gravity * Time.deltaTime;
		RaycastHit fallingHitinfo;
		if(Physics.Raycast(new Ray(transform.position, Vector3.down), out fallingHitinfo)) {
			if(fallingHitinfo.distance < 0.2f){
				state = _state.OnGround;	
				transform.position = fallingHitinfo.point; 
				return;
			}
		}
	}
}
