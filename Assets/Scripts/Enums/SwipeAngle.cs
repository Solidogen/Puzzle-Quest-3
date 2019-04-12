using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeAngle
{
    Up,
    Down,
    Left,
    Right
}

public class SwipeHelpers
{
    public static SwipeAngle getSwipeAngleFromRadian(float radian)
    {
        switch (radian)
        {
            case float r when r > -45 && r <= 45: return SwipeAngle.Right;
            case float r when r > 45 && r <= 135: return SwipeAngle.Up;
            case float r when r > 135 || r <= -135: return SwipeAngle.Left;
            case float r when r < -45 && r >= -135: return SwipeAngle.Down;
            default: throw new InvalidOperationException();
        }
    }
}