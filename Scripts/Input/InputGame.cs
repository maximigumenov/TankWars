using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class InputGame
{
    public static bool Up()
    {
        return Input.GetKey(KeyCode.UpArrow);
    }

    public static bool Down()
    {
        return Input.GetKey(KeyCode.DownArrow);
    }

    public static bool Left()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }

    public static bool Right()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }

    public static bool SelectLeft()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public static bool SelectRight()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public static bool FireDown()
    {
        return Input.GetKeyDown(KeyCode.X);
    }

    public static bool FireUp()
    {
        return Input.GetKeyUp(KeyCode.X);
    }

    public static bool ExitGame()
    {
        return Input.GetKeyUp(KeyCode.Escape);
    }
}

