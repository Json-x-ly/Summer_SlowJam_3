using UnityEngine;
using System.Collections;

public class SectionController : MonoBehaviour {

	public float difficulty;

	private float length;
	private bool isVisible = true;
	private Plane[] frustumPlanes;

	void Awake () {
		Color c;
		c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
			mr.material.color = c;
		}

		Length = GetComponentInChildren<MeshFilter>().mesh.bounds.size.z;
		Length *= transform.Find("ground").localScale.z;
		Length *= transform.localScale.z;
	}

	void Start () {
		
	}

	void Update () {
		frustumPlanes = GeometryUtility.CalculateFrustumPlanes (Camera.main);
		isVisible = false;
		foreach(BoxCollider bc in GetComponentsInChildren<BoxCollider>()) {
			if(GeometryUtility.TestPlanesAABB(frustumPlanes, bc.bounds)) {
				isVisible = true;
				break;
			}
		}
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
