using UnityEngine;
using System.Collections;

public class KillZ : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        foreach (PlayerController pc in PlayerController.players)
        {
            if (pc.transform.position.z < transform.position.z)
            {
                pc.UnloadPostGame();
            }
        }
        if (EggLogic.main.transform.position.z < transform.position.z)
        {
			_root.state = GameState.LOSE;
            EggLogic.main.Reset();
        }
	}
}
