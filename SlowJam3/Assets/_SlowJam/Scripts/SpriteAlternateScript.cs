using UnityEngine;
using System.Collections;

public class SpriteAlternateScript : MonoBehaviour {
	private SpriteRenderer renderer;
	public float framesPerSecond=2.0f;
	public Sprite[] sprites;
	public float t;
	public int index;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		if(t > 1/framesPerSecond)
		{
			t -= 1/framesPerSecond;
			index++;
			if (index > sprites.Length-1)
			{
				index = 0;
			}

			renderer.sprite = sprites[index];
		}
	}
}
