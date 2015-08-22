using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public Vector3 offset;
	public float speed;
	// Use this for initialization
	void Awake () {
        //offset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (_root.state == GameState.PLAYING)
        {
            this.transform.position += Vector3.forward * speed;
        }
	}
}
