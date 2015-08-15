using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    Vector3 offset;
	// Use this for initialization
	void Awake () {
        offset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = PlayerController.playerCenter + offset;
	}
}
