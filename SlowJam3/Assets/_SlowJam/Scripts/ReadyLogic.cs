using UnityEngine;
using System.Collections;

public class ReadyLogic : MonoBehaviour {
    bool isReady = false;
    const float readyPos = 10;
    const float idlePos = 5;
    Vector3 targetPos;
    public bool toggle;
    public int player=-1;
	// Use this for initialization
	void Awake () {
        targetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (toggle)
        {
            toggle = false;
            ToggleReady();
        }
	    if(TinderBox.TinderBoxAPI.ControlDown(player,TinderBox.Controls.Button5)){
            ToggleReady();
        }
        transform.position=Vector3.Lerp(transform.position, targetPos, Time.deltaTime*10);
        
	}
    public void ToggleReady()
    {
        isReady = !isReady;
        if (isReady)
        {
            targetPos.y = readyPos;
        }
        else
        {
            targetPos.y = idlePos;
        }
    }
    public void SetPlayerNumber(int player){
        this.player = player;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material = Resources.Load("Solid"+ LookUp.PlayerColorName(player))as Material;
    }
    public void RemoveReadyCard()
    {
        Debug.Log("removed ready card");
        Destroy(gameObject);
    }
}
