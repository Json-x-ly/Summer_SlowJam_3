using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LogicVisualSpace))]
[RequireComponent(typeof(KillZ))]

public class _root : MonoBehaviour {
    public bool toggle;
    static GameObject go;
    private static Vector3 spawnPos;
    private static bool delayReady=false;
	
	private static IList<StateChangeListener> stateChangeListeners = new List<StateChangeListener>();
	private static GameState myState = GameState.SPLASH;
    public static GameState state
    {
        get
        {
            return myState;
        }
        set
        {
            if (value == myState) return;
            StateChange(myState, value);
           	myState = value;
        }

    }
	private static void StateChange(GameState old, GameState now)
    {
        Debug.Log("State has changed from " + old.ToString() + " to " + now.ToString());
		foreach(StateChangeListener scl in stateChangeListeners) {
			scl.gameStateChanged(old, now);
		}
        switch (now)
        {
            case(GameState.READY):
                LevelController.main.resetLevel();
                SpawnReadyCards();
                if (go.GetComponent<ReadyManager>()==null)
                    go.AddComponent<ReadyManager>();
                go.transform.position = spawnPos;
                break;
            case(GameState.PLAYING):
                PlayerManager.PrepPlayers();
                break;
			/*
            case (GameState.LOSE):
                delayReady = true;
                break;
            case (GameState.WIN):
                delayReady = true;
                break;
                */
        }
        switch (old)
        {
            case(GameState.READY):
                ReadyLogic[] scripts = FindObjectsOfType<ReadyLogic>();
                foreach (ReadyLogic script in scripts)
                {
                    script.RemoveReadyCard();
                }
                Destroy(go.GetComponent<ReadyManager>());
                break;
			/*
            case (GameState.PLAYING):
                PlayerManager.CleanUpAllPlayers();
                break;
                */
        }
    }
	public static void addStateChangeListener(StateChangeListener scl) {
		stateChangeListeners.Add(scl);
	}
    void Awake()
    {
        go = this.gameObject;
        spawnPos = transform.position;
        MeshFilter[] meshes = GameObject.FindObjectsOfType(typeof(MeshFilter))as MeshFilter[];
        Debug.Log("Meshes found: " + meshes.Length);
        foreach (MeshFilter mesh in meshes)
        {
            Vector2[] uvs = new Vector2[mesh.mesh.vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0.5f,0.5f);
            }
            mesh.mesh.uv = uvs;
        }
	}
    void Start()
    {

        state = GameState.READY;
    }
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            state = GameState.PLAYING;
        }
        if (toggle)
        {
            toggle = false;
            state = GameState.PLAYING;
        }
        if (delayReady)
        {
            delayReady = false;
            state = GameState.READY;
        }
    }
    static readonly Vector3 rcStartPos = new Vector3(-5, 5, 1);
    static readonly Vector3 rcSize = new Vector3(3, 5, 1);
    const float rcStepLength = 3+1.0f / 3.0f;
    public static void SpawnReadyCards()
    {
        for (int x = 0; x < 4; x++)
        {
            int cabPos = LookUp.PlayerCabinetPosition(x);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "ReadyPlayer " + x;
            go.transform.position = rcStartPos + Vector3.right * cabPos * rcStepLength;
            go.transform.localScale = rcSize;
            go.layer = LayerMask.NameToLayer("UI");
            ReadyLogic script = go.AddComponent<ReadyLogic>();
            script.SetPlayerNumber(x);
        }
    }
}
