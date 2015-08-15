using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SectionManagerBehaviour : MonoBehaviour {
	public GameObject player;

	private ArrayList currentPath = new ArrayList();
	private SortedList<float, GameObject> sections;
	private float pathLength;
	private float speed = 2f;

	void Awake() {
		player = GameObject.Find("Player");
	}

	void Start () {
		GameObject[] loadedSections = (GameObject[]) Resources.LoadAll ("Sections", typeof(GameObject)).Cast<GameObject>().ToArray();
		sections = new SortedList<float, GameObject>(loadedSections.Length);
		foreach (GameObject go in loadedSections) {
			float difficulty = go.GetComponent<SectionController>().difficulty;
			sections.Add(difficulty, go);
		}

		float length = 0f;
		for (int i = 0; i < 5; i++) {
			GameObject section = GetSectionForDifficulty(CalculateDifficulty(length));
			Vector3 pos = Vector3.zero;
			if(i > 0) {
				GameObject last = currentPath[i-1] as GameObject;
				pos = last.transform.position;
				pos.z += last.GetComponent<SectionController>().Length/2;
			}
			GameObject newObject = (GameObject) Instantiate(section, pos, Quaternion.identity);
			float newObjectLength = newObject.GetComponent<SectionController>().Length;
			pos.z += newObjectLength/2;
			pathLength += newObjectLength;
			newObject.transform.position = pos;
			currentPath.Add(newObject);
		}
	}

	void Update () {
		Vector3 pos = player.transform.position;
		pos.z += speed * Time.deltaTime;
		player.transform.position = pos;

		GameObject firstSection = (GameObject)currentPath [0];
		if(!firstSection.GetComponent<SectionController>().IsVisible) {
			currentPath.RemoveAt(0);
			Destroy(firstSection);
			addNextSection();
		}
	}

	private void addNextSection() {
		GameObject nextSection = GetSectionForDifficulty(CalculateDifficulty(pathLength));
		GameObject lastSection = ((GameObject)currentPath [currentPath.Count-1]);
		Vector3 pos = lastSection.transform.position;
		pos.z += lastSection.GetComponent<SectionController>().Length/2;
		GameObject newObject = (GameObject) Instantiate(nextSection, pos, Quaternion.identity);
		float newObjectLength = newObject.GetComponent<SectionController>().Length;
		pos.z += newObjectLength/2;
		pathLength += newObjectLength;
		newObject.transform.position = pos;
		currentPath.Add(newObject);
	}

	//TODO: this
	private float CalculateDifficulty(float length) {
		return .5f;
	}

	private GameObject GetSectionForDifficulty(float difficulty) {
		List<float> keys = new List<float>(sections.Keys);
		int i = keys.BinarySearch (difficulty);
		if (i < 0)
			i = ~i - 1;
		if (i == -1)
			i = 0;
		return sections.Values[i];
	}
}
