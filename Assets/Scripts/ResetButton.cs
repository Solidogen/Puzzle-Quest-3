using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void ResetBoard()
    {
        FindObjectOfType<Board>()?.Also(board => {
            board.doForEveryDot((c, r) => {
                board.allDotsOnBoard[c, r].GetComponent<Dot>().isMatched = true;
            });
            board.DestroyAllMatches();
        });
    }
}
