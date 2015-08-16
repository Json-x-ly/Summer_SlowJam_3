using UnityEngine;
using System.Collections;

public class SectionController : MonoBehaviour {

	public float difficulty;

	private float length;
	private bool isVisible = true;
	private Plane[] frustumPlanes;

	void Awake () {
		Color c;
		float min = difficulty - .1f;
		float max = difficulty + .1f;
		if (min < 0f)
			min = 0f;
		if (max > 1f)
			max = 1f;
		c = new Color(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
		foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
			mr.material.color = c;
		}
	
		//this will be cleaner once we get rid of the place holder tiles
		Transform t = transform.FindChild ("Mesh").FindChild("tileMesh");
		if (t != null) {
			Length += t.GetComponent<MeshFilter> ().mesh.bounds.size.z;
			Length *= t.localScale.z;
		} else {
			Length = GetComponentInChildren<MeshFilter> ().mesh.bounds.size.z;
			Length *= transform.FindChild("Mesh").localScale.z;
		}
		Length *= transform.localScale.z;
	}

	void Start () {
		
	}

	void Update () {
		/*maybe we'll use this one day
		frustumPlanes = GeometryUtility.CalculateFrustumPlanes (Camera.main);
		isVisible = false;
		foreach(MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			if(GeometryUtility.TestPlanesAABB(frustumPlanes, mc.bounds)) {
				isVisible = true;
				break;
			}
		}
		*/
	}

	public bool IsVisible {
		get {
			return isVisible;
		}
		set {
			isVisible = value;
		}
	}

	public float Length {
		get {
			return length;
		}
		set {
			length = value;
		}
	}
}
