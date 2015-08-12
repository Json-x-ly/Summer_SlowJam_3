using UnityEngine;
using System.Collections;

namespace TinderBox
{
    public static partial class TinderBoxAPI
    {
        /// <summary>
        /// Call this when the game is fully loaded and ready to present to the player.
        /// </summary>
        public static void IsReady() { _IsReady(); }

        /// <summary>
        /// Call this when the game is ready to be hidden and returned to the Tinderbox main menu.
        /// </summary>
        public static void GameOver() { _GameOver(); }

        /// <summary>
        /// Find the up/down state of the specified control.
        /// </summary>
        /// <param name="player">The player who owns the control.</param>
        /// <param name="control">The control in question.</param>
        /// <returns>True if the control is pressed, false if it is not pressed.</returns>
        public static bool ControlState(int player, Controls control) { return _ControlState(player, control); }
        public static bool ControlState(Players player, Controls control) { return _ControlState(player, control); }

        /// <summary>
        /// Determine if the state of a specified control is currently up.
        /// </summary>
        /// <param name="player">The player who owns the control.</param>
        /// <param name="control">The control in question.</param>
        /// <returns>True if the control is not pressed, false if it is pressed.</returns>
        public static bool ControlUp(int player, Controls control) { return _ControlUp(player, control); }
        public static bool ControlUp(Players player, Controls control) { return _ControlUp(player, control); }

        /// <summary>
        /// Determine if the state of a specified control is currently down.
        /// </summary>
        /// <param name="player">The player who owns the control.</param>
        /// <param name="control">The control in question.</param>
        /// <returns>True if the control is pressed, false if it is not pressed.</returns>
        public static bool ControlDown(int player, Controls control) { return _ControlDown(player, control); }
        public static bool ControlDown(Players player, Controls control) { return _ControlDown(player, control); }

        enum KeyMapping
        {
            Player1Up = KeyCode.UpArrow,
            Player1Down = KeyCode.DownArrow,
            Player1Left = KeyCode.LeftArrow,
            Player1Right = KeyCode.RightArrow,
            Player1Button1 = KeyCode.M,
            Player1Button2 = KeyCode.Comma,
            Player1Button3 = KeyCode.Period,
            Player1Button4 = KeyCode.Slash,
            Player1Button5 = KeyCode.RightShift,
            Player1Start = KeyCode.Return,

            Player2Up = KeyCode.W,
            Player2Down = KeyCode.S,
            Player2Left = KeyCode.A,
            Player2Right = KeyCode.D,
            Player2Button1 = KeyCode.V,
            Player2Button2 = KeyCode.G,
            Player2Button3 = KeyCode.Y,
            Player2Button4 = KeyCode.C,
            Player2Button5 = KeyCode.F,
            Player2Start = KeyCode.T,

            Player3Up = KeyCode.U,
            Player3Down = KeyCode.J,
            Player3Left = KeyCode.H,
            Player3Right = KeyCode.K,
            Player3Button1 = KeyCode.O,
            Player3Button2 = KeyCode.P,
            Player3Button3 = KeyCode.L,
            Player3Button4 = KeyCode.Semicolon,
            Player3Button5 = KeyCode.Quote,
            Player3Start = KeyCode.LeftBracket,

            Player4Up = KeyCode.Alpha2,
            Player4Down = KeyCode.Alpha3,
            Player4Left = KeyCode.Alpha1,
            Player4Right = KeyCode.Alpha4,
            Player4Button1 = KeyCode.Alpha5,
            Player4Button2 = KeyCode.Alpha6,
            Player4Button3 = KeyCode.Alpha7,
            Player4Button4 = KeyCode.Alpha8,
            Player4Button5 = KeyCode.Alpha9,
            Player4Start = KeyCode.Alpha0,
        }
    }

    public enum Players
    {
        Player1,
        Player2,
        Player3,
        Player4,
        AnyPlayer
    }

    public enum Controls
    {
        Up,
        Down,
        Left,
        Right,
        Button1,
        Button2,
        Button3,
        Button4,
        Button5,
        Start
    }
}
