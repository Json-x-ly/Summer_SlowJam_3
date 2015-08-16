using UnityEngine;
using System.Collections;

public class LookUp : MonoBehaviour {

	private static int[]     _positions      = {1,           2,              0,          3};
    private static Color[]   _colors         = {Color.red,   Color.yellow,   Color.blue, Color.green};
    private static string[]  _colorNames     = { "Red",      "Yellow",       "Blue",     "Green" };

    /// <summary>
    /// Gets the players postion in the cabnet starting at 0 on the left.
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public static int PlayerCabinetPosition(int playerNumber)
    {
        return _positions[playerNumber];
    }
    /// <summary>
    /// Gets the Player number from Cabnet position
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public static int PlayerLogicPosition(int playerNumber)
    {
        foreach (int pos in _positions)
        {
            if (_positions[pos] == playerNumber)
                return pos;
        }
        return -1;
    }
    /// <summary>
    /// Gets the Color assigned to that player
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public static Color PlayerColor(int playerNumber)
    {
        return _colors[playerNumber];
    }
    /// <summary>
    /// Gets the string of the Color assigned to that player.
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public static string PlayerColorName(int playerNumber)
    {
        return _colorNames[playerNumber];
    }
}
