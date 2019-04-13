using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;

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
        board.doForEveryDot((i, j) =>
        {
            GameObject currentDot = board.allDotsOnBoard[i, j];
            currentDot?.Also(dot =>
            {
                if (i > 0 && i < board.width - 1)
                {
                    var leftDot = board.allDotsOnBoard[i - 1, j];
                    var rightDot = board.allDotsOnBoard[i + 1, j];
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
                if (j > 0 && j < board.height - 1)
                {
                    var upDot = board.allDotsOnBoard[i, j + 1];
                    var downDot = board.allDotsOnBoard[i, j - 1];
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
}
