using UnityEngine;
using System.Collections;

public class SpriteAlternateScript : MonoBehaviour {
	private SpriteRenderer renderer;
	private QTEScript QTE;
	public float framesPerSecond=2.0f;
	public Sprite[] sprites;
	public float t;
	public int index;
	public Color color;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer> ();
		QTE = GetComponentInParent<QTEScript> ();
		float rand = Random.Range (0, 4);
		if (rand > 3)
		{
			color = Color.blue;
			QTE.buttonQTE = TinderBox.Controls.Button1;
		}else if (rand > 2)
		{
			color = Color.red;
			QTE.buttonQTE = TinderBox.Controls.Button2;
		}else if (rand > 1)
		{
			color = Color.yellow;
			QTE.buttonQTE = TinderBox.Controls.Button3;
		}else 
		{
			color = Color.green;
			QTE.buttonQTE = TinderBox.Controls.Button4;
		}
		renderer.color = color;
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
