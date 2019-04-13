using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DotType
{
    Normal,
    RowBomb,
    ColumnBomb,
    ColorBomb
}

public class DotTypeHelpers {
    public static DotType GetRandomDirectionBomb()
    {
        var bombs = new List<DotType>() {DotType.RowBomb, DotType.ColumnBomb};
        return bombs[Random.Range(0, bombs.Count)];
    }
}