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
                if (c > 0 && c < board.width - 1)
                {
                    var leftDot = board.allDotsOnBoard[c - 1, r];
                    var rightDot = board.allDotsOnBoard[c + 1, r];

                    if (dot.GetComponent<Dot>().dotType == DotType.RowBomb
                        || leftDot.GetComponent<Dot>().dotType == DotType.RowBomb
                        || rightDot.GetComponent<Dot>().dotType == DotType.RowBomb)
                    {
                        var rowDots = GetDotsForRow(r);
                        rowDots.ForEach(rowDot => {
                            rowDot.GetComponent<Dot>().isMatched = true;
                        });
                        currentMatches.Union(rowDots);
                    }

                    if (leftDot != null && rightDot != null && leftDot.tag == dot.tag && rightDot.tag == dot.tag)
                    {
                        if (!currentMatches.Contains(leftDot))
                        {
                            currentMatches.Add(leftDot);
                        }
                        if (!currentMatches.Contains(rightDot))
                        {
                            currentMatches.Add(rightDot);
                        }
                        if (!currentMatches.Contains(dot))
                        {
                            currentMatches.Add(dot);
                        }
                        leftDot.GetComponent<Dot>().isMatched = true;
                        rightDot.GetComponent<Dot>().isMatched = true;
                        dot.GetComponent<Dot>().isMatched = true;
                    }
                }
                if (r > 0 && r < board.height - 1)
                {
                    var upDot = board.allDotsOnBoard[c, r + 1];
                    var downDot = board.allDotsOnBoard[c, r - 1];
                    if (upDot != null && downDot != null && upDot.tag == dot.tag && downDot.tag == dot.tag)
                    {
                        if (!currentMatches.Contains(upDot))
                        {
                            currentMatches.Add(upDot);
                        }
                        if (!currentMatches.Contains(downDot))
                        {
                            currentMatches.Add(downDot);
                        }
                        if (!currentMatches.Contains(dot))
                        {
                            currentMatches.Add(dot);
                        }
                        upDot.GetComponent<Dot>().isMatched = true;
                        downDot.GetComponent<Dot>().isMatched = true;
                        dot.GetComponent<Dot>().isMatched = true;
                    }
                }
            });
        });
    }

    List<GameObject> GetDotsForColumn(int column)
    {
        var dots = new List<GameObject>();
        board.doForColumn(column, (c, r) => {
            if (board.allDotsOnBoard[c, r] != null)
            {
                dots.Add(board.allDotsOnBoard[c, r]);
            }
        });
        return dots;
    }

    List<GameObject> GetDotsForRow(int row)
    {
        var dots = new List<GameObject>();
        board.doForRow(row, (c, r) => {
            if (board.allDotsOnBoard[c, r] != null)
            {
                dots.Add(board.allDotsOnBoard[c, r]);
            }
        });
        return dots;
    }
}
