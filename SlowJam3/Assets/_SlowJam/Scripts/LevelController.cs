using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelController : MonoBehaviour {
	public GameObject player;
	public GameObject endCave;
    public static LevelController main;
	public float gameLength;

	private ArrayList currentPath = new ArrayList();
	private SortedList<float, GameObject> dynamicSections;
	private GameObject[] staticSections;
	private bool levelDone = false;
	private float currentLength = 0;
	public float speed = 30f;
	private int sectionsSinceDynamic = 0; //how many  sections have been spawned that weren't dynamic

	public void resetLevel() {
		currentPath.Clear();
		levelDone = false;
		currentLength = 0;
		sectionsSinceDynamic = 0;
		spawnInitialSections();
	}

	void Awake() {
		//player = GameObject.Find("Player");
        main = this;
        GameObject[] loadedDynamicSections = Resources.LoadAll("Sections/Dynamic", typeof(GameObject)).Cast<GameObject>().ToArray();
        dynamicSections = new SortedList<float, GameObject>(loadedDynamicSections.Length);
        foreach (GameObject go in loadedDynamicSections)
        {
            float difficulty = go.GetComponent<SectionController>().difficulty;
            dynamicSections.Add(difficulty, go);
        }

        staticSections = Resources.LoadAll("Sections/Static", typeof(GameObject)).Cast<GameObject>().ToArray();
        spawnInitialSections();
	}

	private void spawnInitialSections() {
		for (int i = 0; i < 15; i++) {
			GameObject section = GetSectionForDifficulty(CalculateDifficulty(currentLength));
			Vector3 pos = Vector3.zero;
			if(i > 0) {
				GameObject last = currentPath[i-1] as GameObject;
				pos = last.transform.position;
				pos.z += last.GetComponent<SectionController>().Length/2;
			}
			GameObject newObject = (GameObject) Instantiate(section, pos, Quaternion.identity);
			float newObjectLength = newObject.GetComponent<SectionController>().Length;
			pos.z += newObjectLength/2;
			currentLength += newObjectLength;
			newObject.transform.position = pos;
			currentPath.Add(newObject);
		}
	}

	void Update () {
		if (_root.state != GameState.PLAYING)
			return;
		GameObject firstSection = (GameObject) currentPath [0];
		if(Camera.main.transform.position.z - 15 > firstSection.transform.position.z) {
			currentPath.RemoveAt(0);
			Destroy(firstSection);
			if (levelDone)
				return;
			if(currentLength < gameLength) {
				addNextSection();
			} else {
				addEndingSection();
				levelDone = true;
			}
		}
	}

	private void addNextSection() {
		GameObject nextSection = GetSectionForDifficulty(CalculateDifficulty(currentLength));
		GameObject lastSection = ((GameObject) currentPath [currentPath.Count-1]);
		Vector3 pos = lastSection.transform.position;
		pos.z += lastSection.GetComponent<SectionController>().Length/2;
		GameObject newObject = (GameObject) Instantiate(nextSection, pos, Quaternion.identity);
		float newObjectLength = newObject.GetComponent<SectionController>().Length;
		pos.z += newObjectLength/2;
		newObject.transform.position = pos;
		currentLength += newObjectLength;
		currentPath.Add(newObject);
	}

	private void addEndingSection() {
		GameObject lastSection = ((GameObject) currentPath [currentPath.Count-1]);
		Vector3 pos = lastSection.transform.position;
		pos.z += lastSection.GetComponent<SectionController>().Length/2;
		GameObject caveInstance = (GameObject) Instantiate(endCave, pos, Quaternion.identity);
		caveInstance.transform.position = pos;
	}

	//TODO: this
	private float CalculateDifficulty(float length) {
		float perc = currentLength / gameLength;
		float range = perc * perc * .5f;
		float min = perc - range;
		float max = perc + range;
		if (min < 0f)
			min = 0f;
		if (max > 1f)
			max = 1f;
		float difficulty = Random.Range(min, max);
		//Debug.Log ("diff: " + difficulty + ", perc: " + perc + ", range: " + range);
		return difficulty;
	}

	private GameObject GetSectionForDifficulty(float difficulty) {
		bool spawnDynamic = Random.Range (0, 3) < sectionsSinceDynamic;
		if (spawnDynamic) {
			List<float> keys = new List<float> (dynamicSections.Keys);
			int i = keys.BinarySearch (difficulty);
			if (i < 0)
				i = ~i - 1;
			if (i == -1)
				i = 0;
			sectionsSinceDynamic = 0;
			return dynamicSections.Values [i];
		} else {
			sectionsSinceDynamic++;
			return staticSections[Mathf.RoundToInt(Random.Range(0f, (float) staticSections.Count()-1))];
		}
	}
}
