using UnityEngine;
using System.Collections;

public class ReadyManager : MonoBehaviour {
    int lastChangedPlayer = -1;
    bool is2Players = false;
    float startAtTime=float.MaxValue;
    public float timeLeft;
	void Update () {
        ///for debugging
        timeLeft = startAtTime - Time.time;
        if (PlayerManager.playerCount > 1)
        {
            if (!is2Players)
            {
                startAtTime = Time.time + 10;
                is2Players = true;
            }
            if (PlayerManager.activePlayers != lastChangedPlayer)
            {
                if (startAtTime < Time.time + 3)
                {
                    startAtTime = Time.time + 3;
                }
                lastChangedPlayer = PlayerManager.activePlayers;
            }
        }
        else
        {
            is2Players = false;
            startAtTime = float.MaxValue;
        }
        if (startAtTime < Time.time && startAtTime > Time.time- 10)
        {
			_root.state = GameState.PLAYING;
        }

	}
    void ResetTimerLong()
    {

    }
}
