using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        board.doForEveryDot((c, r) =>
        {
            GameObject currentDot = board.allDotsOnBoard[c, r];
            currentDot?.Also(dot =>
            {
                var currentDotScript = dot.GetComponent<Dot>();
                if (c > 0 && c < board.width - 1)
                {
                    var leftDot = board.allDotsOnBoard[c - 1, r];
                    var rightDot = board.allDotsOnBoard[c + 1, r];

                    if (leftDot != null && rightDot != null && leftDot.tag == dot.tag && rightDot.tag == dot.tag)
                    {
                        var leftDotScript = leftDot.GetComponent<Dot>();
                        var rightDotScript = rightDot.GetComponent<Dot>();
                        currentMatches.Union(IsRowBomb(leftDotScript, currentDotScript, rightDotScript));
                        currentMatches.Union(IsColumnBomb(leftDotScript, currentDotScript, rightDotScript));
                        currentMatches.Union(IsAdjacentBomb(leftDotScript, currentDotScript, rightDotScript));
                        GetNearbyPieces(leftDot, currentDot, rightDot);
                    }
                }
                if (r > 0 && r < board.height - 1)
                {
                    var upDot = board.allDotsOnBoard[c, r + 1];
                    var downDot = board.allDotsOnBoard[c, r - 1];

                    if (upDot != null && downDot != null && upDot.tag == dot.tag && downDot.tag == dot.tag)
                    {
                        var upDotScript = upDot.GetComponent<Dot>();
                        var downDotScript = downDot.GetComponent<Dot>();
                        currentMatches.Union(IsColumnBomb(upDotScript, currentDotScript, downDotScript));
                        currentMatches.Union(IsRowBomb(upDotScript, currentDotScript, downDotScript));
                        currentMatches.Union(IsAdjacentBomb(upDotScript, currentDotScript, downDotScript));
                        GetNearbyPieces(upDot, currentDot, downDot);
                    }
                }
            });
        });
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //Check if the piece is inside the board
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    dots.Add(board.allDotsOnBoard[i, j]);
                    board.allDotsOnBoard[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        }
        return dots;
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDotsOnBoard[column, i] != null)
            {
                dots.Add(board.allDotsOnBoard[column, i]);
                board.allDotsOnBoard[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDotsOnBoard[i, row] != null)
            {
                dots.Add(board.allDotsOnBoard[i, row]);
                board.allDotsOnBoard[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    public void CheckDirectionalBombs()
    {
        if (board.currentDot == null)
        {
            return;
        }
        if (board.currentDot.isMatched)
        {
            // unmatch and swap for a bomb
            board.currentDot.MakeBomb(DotTypeHelpers.GetRandomDirectionalBomb());
        }
        else if (board.currentDot.otherDot != null)
        {
            var otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            if (otherDot.isMatched)
            {
                otherDot.MakeBomb(DotTypeHelpers.GetRandomDirectionalBomb());
            }
        }
    }

    public void MatchDotsOfColor(string color)
    {
        board.doForEveryDot((c, r) =>
        {
            if (board.allDotsOnBoard[c, r] != null && board.allDotsOnBoard[c, r].tag == color)
            {
                board.allDotsOnBoard[c, r].GetComponent<Dot>().isMatched = true;
            }
        });
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType == DotType.AdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.dotType == DotType.AdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.dotType == DotType.AdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType == DotType.RowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.dotType == DotType.RowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.dotType == DotType.RowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.dotType == DotType.ColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.dotType == DotType.ColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.dotType == DotType.ColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
}
