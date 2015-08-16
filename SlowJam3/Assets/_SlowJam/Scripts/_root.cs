using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LogicVisualSpace))]
public class _root : MonoBehaviour {
    public bool toggle;
    static GameObject go;
    public enum _state { SplashScreen, Ready, Playing, Lose };
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
                SpawnReadyCards();
                go.AddComponent<ReadyManager>();
                break;
            case(_state.Playing):
                PlayerManager.PrepPlayers();
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
        }
    }
    void Awake()
    {
        go = this.gameObject;
        state = _state.Ready;
        //TinderBox.TinderBoxAPI.IsReady();
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
    }
    static readonly Vector3 rcStartPos = new Vector3(-5, 5, 1);
    static readonly Vector3 rcSize = new Vector3(3, 5, 1);
    const float rcStepLength = 3+1.0f / 3.0f;
    public static void SpawnReadyCards()
    {
        for (int x = 0; x < 4; x++)
        {
            int cabPos = LookUp.PlayerPosition(x);
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
