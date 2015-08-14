using UnityEngine;
using System.Collections;

public class _root : MonoBehaviour {
    public enum _state { SplashScreen, Ready, Player, Lose };
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

                break;
        }
    }
	// Use this for initialization
	void Awake () {
        //TinderBox.TinderBoxAPI.IsReady();
        state = _state.Ready;
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
            ReadyLogic script = go.AddComponent<ReadyLogic>();
            script.SetPlayerNumber(x);
        }
    }
}
