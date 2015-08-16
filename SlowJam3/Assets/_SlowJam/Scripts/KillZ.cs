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
	}
}
