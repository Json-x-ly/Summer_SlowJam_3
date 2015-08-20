using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LogicVisualSpace))]
[RequireComponent(typeof(KillZ))]

public class _root : MonoBehaviour {
    public bool toggle;
    static GameObject go;
    private static Vector3 spawnPos;
    private static bool delayReady=false;
    public enum _state { SplashScreen, Ready, Playing, Lose ,Win};
    private static _state __state = _state.SplashScreen;
    public static _state state
    {
        get
        {
            return __state;
        }
        set
        {
            if (value == __state) return;
            StateChange(__state, value);
            __state = value;
        }

    }
    private static void StateChange(_state old, _state now)
    {
        Debug.Log("State has changed from " + old.ToString() + " to " + now.ToString());
        switch (now)
        {
            case(_state.Ready):
                LevelController.main.resetLevel();
                SpawnReadyCards();
                if (go.GetComponent<ReadyManager>()==null)
                    go.AddComponent<ReadyManager>();
                go.transform.position = spawnPos;
                break;
            case(_state.Playing):
                PlayerManager.PrepPlayers();
                break;
            case (_state.Lose):
                TinderBox.TinderBoxAPI.GameOver();
                delayReady = true;
                break;
            case (_state.Win):
                delayReady = true;
                break;
        }
        switch (old)
        {
            case(_state.Ready):
                ReadyLogic[] scripts = FindObjectsOfType<ReadyLogic>();
                foreach (ReadyLogic script in scripts)
                {
                    script.RemoveReadyCard();
                }
                Destroy(go.GetComponent<ReadyManager>());
                break;
            case (_state.Playing):
                PlayerManager.CleanUpAllPlayers();
                break;
        }
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
        
        TinderBox.TinderBoxAPI.IsReady();
	}
    void Start()
    {

        state = _state.Ready;
    }
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            state = _state.Playing;
        }
        if (toggle)
        {
            toggle = false;
            state = _state.Playing;
        }
        if (delayReady)
        {
            delayReady = false;
            state = _state.Ready;
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
