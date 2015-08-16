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
        //Changed from rendering color to setting UV space;
		foreach(MeshFilter mf in GetComponentsInChildren<MeshFilter>()) {
			//mr.material.color = c;

            Vector2[] uvs = new Vector2[mf.mesh.vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0.5f, 0.5f);
            }
            mf.mesh.uv = uvs;
		}

		Length = GetComponentInChildren<MeshFilter>().mesh.bounds.size.z;
		//Length *= transform.Find("Mesh").localScale.z;
		Length *= transform.localScale.z;
	}

	void Start () {
		
	}

	void Update () {
		frustumPlanes = GeometryUtility.CalculateFrustumPlanes (Camera.main);
		isVisible = false;
		foreach(MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			if(GeometryUtility.TestPlanesAABB(frustumPlanes, mc.bounds)) {
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
