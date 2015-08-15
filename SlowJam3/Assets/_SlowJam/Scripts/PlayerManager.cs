using UnityEngine;
using System.Collections;

public class PlayerManager{
    const int PLAYER1 = 1;
    const int PLAYER2 = 2;
    const int PLAYER3 = 4;
    const int PLAYER4 = 8;
    private static PlayerController[] registerdPlayers = new PlayerController[4];
    private static int _playerCount;
    public static int playerCount
    {
        get { return _playerCount; }
    }
    private static int activePlayer = 0;

    /// <summary>
    /// Set a player to be in the game
    /// </summary>
    /// <param name="number"></param>
    public static void SetPlayerToActive(int number)
    {
        activePlayer = activePlayer | (1 << number);
        UpdatePlayerCount();
    }
    /// <summary>
    /// Removes Player from active list
    /// </summary>
    /// <param name="number"></param>
    public static void RemovedPlayerFromActive(int number)
    {
        activePlayer = ~(~activePlayer | (1 << number));
        UpdatePlayerCount();
    }
    /// <summary>
    /// Check if the player is in the game;
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static bool IsPlayerActive(int number)
    {
        return (activePlayer & (1 << number))>0;
    }
    private static void UpdatePlayerCount(){
        int temp = 0;
        for (int x = 0; x < 4; x++)
        {
            if (IsPlayerActive(x))
            {
                temp++;
            }
        }
        _playerCount=temp;
        Debug.Log(playerCount + ", " + activePlayer);
    }
    public static int InitPlayerRegistration(PlayerController pc)
    {
        for (int x = 0; x < 4; x++)
        {
            if (registerdPlayers[x] == null)
            {
                registerdPlayers[x] = pc;
                return x;
            }
        }
        Debug.LogError("To many registering players");
        return -1;
    }
    public static void PrepPlayers()
    {
        for (int x = 0; x < 4; x++)
        {
            if (IsPlayerActive(x))
            {
                Debug.Log("PLayer " + x + " is ready.");
                registerdPlayers[x].PrepForGame();
            }
        }
    }

}
