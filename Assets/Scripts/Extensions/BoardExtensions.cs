using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardExtensions
{
    public static void doForEveryDot(this Board board, Action<int, int> mainAction,
        Action afterEachColumnAction = null)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                mainAction.Invoke(i, j);
            }
            afterEachColumnAction?.Invoke();
        }
    }
}
