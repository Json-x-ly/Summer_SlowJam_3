using UnityEngine;
using System.Collections;

public interface StateChangeListener {

	void playerStateChanged (int playerIndex, PlayerState oldState, PlayerState newState);

	void gameStateChanged(GameState oldState, GameState newState);
}
