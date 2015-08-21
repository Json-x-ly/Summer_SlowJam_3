using UnityEngine;
using System.Collections;

public class SectionController : MonoBehaviour {

	public float difficulty;

	private float length;
	private Plane[] frustumPlanes;

	void Awake () {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = LogicVisualSpace.main.worldMat;
        }
		foreach(MeshFilter mf in GetComponentsInChildren<MeshFilter>()) {
            
            Vector2[] uvs = new Vector2[mf.mesh.vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0.5f, 0.5f);
            }
            mf.mesh.uv = uvs;
		}
	
		//this will be cleaner once we get rid of the place holder tiles
		//Transform t = transform.FindChild ("Mesh").FindChild("tileMesh");
		//if (t != null) {
		//	Length += t.GetComponent<MeshFilter> ().mesh.bounds.size.z;
		//	Length *= t.localScale.z;
		//} else {
		//	Length = GetComponentInChildren<MeshFilter> ().mesh.bounds.size.z;
		//	Length *= transform.FindChild("Mesh").localScale.z;
		//}
		Length = 15;
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

	void OnTriggerEnter(Collider other) { //only the end section has a collider
		PlayerController pc = other.GetComponent<PlayerController> ();
		if(pc == null)
			return;
		if (pc.state == PlayerState.HOLDING) {
			Debug.Log ("the egg made it to the cave");
			_root.state = GameState.WIN;
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
