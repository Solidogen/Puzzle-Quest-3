using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardExtensions
{
    public static void doForEveryDot(this Board board, Action<int, int> mainAction,
        Action afterEachColumnAction = null)
    {
        for (int column = 0; column < board.width; column++)
        {
            for (int row = 0; row < board.height; row++)
            {
                mainAction.Invoke(column, row);
            }
            afterEachColumnAction?.Invoke();
        }
    }

    public static void doForColumn(this Board board, int column, Action<int, int> action)
    {
        for (int row = 0; row < board.height; row++)
        {
            action.Invoke(column, row);
        }
    }

    public static void doForRow(this Board board, int row, Action<int, int> action)
    {
        for (int column = 0; column < board.width; column++)
        {
            action.Invoke(column, row);
        }
    }
}
