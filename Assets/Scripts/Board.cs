using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    private FindMatches findMatches;
    public GameState currentState = GameState.Move;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    public GameObject[] availableDotColors;
    public GameObject[,] allDotsOnBoard;
    private BackgroundTile[,] allBackgroundTiles;
    public GameObject destroyEffect;
    public Dot currentDot;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allBackgroundTiles = new BackgroundTile[width, height];
        allDotsOnBoard = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        this.doForEveryDot((c, r) =>
        {
            var cellName = $"( {c}, {r} )";

            var tempPosition = new Vector2(c, r + offset);
            var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
            backgroundTile.name = "bg";

            int dotToUse = Random.Range(0, availableDotColors.Length);

            int maxIterations = 0;
            while (MatchesAt(c, r, availableDotColors[dotToUse]) && maxIterations < 100)
            {
                dotToUse = Random.Range(0, availableDotColors.Length);
                maxIterations++;
            }

            GameObject dot = Instantiate(availableDotColors[dotToUse], tempPosition, Quaternion.identity);
            dot.GetComponent<Dot>().row = r;
            dot.GetComponent<Dot>().column = c;
            dot.transform.parent = transform;
            dot.name = "dot" + cellName;
            backgroundTile.transform.parent = dot.transform;

            allDotsOnBoard[c, r] = dot;
        });
    }


    private bool MatchesAt(int column, int row, GameObject gameObject)
    {
        if (column > 1 && row > 1)
        {
            if (allDotsOnBoard[column - 1, row].tag == gameObject.tag &&
                allDotsOnBoard[column - 2, row].tag == gameObject.tag)
            {
                return true;
            }
            if (allDotsOnBoard[column, row - 1].tag == gameObject.tag &&
                allDotsOnBoard[column, row - 2].tag == gameObject.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDotsOnBoard[column, row - 1].tag == gameObject.tag &&
                    allDotsOnBoard[column, row - 2].tag == gameObject.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDotsOnBoard[column - 1, row].tag == gameObject.tag &&
                    allDotsOnBoard[column - 2, row].tag == gameObject.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void DestroyAllMatches()
    {
        this.doForEveryDot((c, r) =>
        {
            DestroyMatchesAt(c, r);
        });
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCoroutine());
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDotsOnBoard[column, row] != null && allDotsOnBoard[column, row].GetComponent<Dot>().isMatched)
        {
            // how many elements are in the matched pieces list?
            if (findMatches.currentMatches.Count > 3)
            {
                CheckIfNeedToMakeBombs();
            }

            GameObject particle = Instantiate(destroyEffect, allDotsOnBoard[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDotsOnBoard[column, row]);
            allDotsOnBoard[column, row] = null;
        }
    }

    private void CheckIfNeedToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckDirectionalBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (IsFiveOfAKindInStraightLine())
            {
                MakeOneOfSwappedDotsABomb(currentDot, DotType.ColorBomb);
            }
            else
            {
                MakeOneOfSwappedDotsABomb(currentDot, DotType.AdjacentBomb);
            }
        }
    }

    private void MakeOneOfSwappedDotsABomb(Dot currentDot, DotType bombType)
    {
        currentDot?.Also(dot =>
        {
            if (dot.isMatched)
            {
                dot.MakeBomb(bombType);
            }
            else
            {
                currentDot.otherDot?.GetComponent<Dot>()?.Also(other =>
                {
                    if (other.isMatched)
                    {
                        other.MakeBomb(bombType);
                    }
                });
            }
        });
    }

    private bool IsFiveOfAKindInStraightLine()
    {
        int horizontalDotsCount = 0;
        int verticalDotsCount = 0;
        findMatches.currentMatches[0].GetComponent<Dot>()?.Also(firstDot =>
        {
            foreach (var dot in findMatches.currentMatches)
            {
                var dotScript = dot.GetComponent<Dot>();
                if (dotScript.row == firstDot.row)
                {
                    horizontalDotsCount++;
                }
                if (dotScript.column == firstDot.column)
                {
                    verticalDotsCount++;
                }
            }
        });
        return horizontalDotsCount == 5 || verticalDotsCount == 5;
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        this.doForEveryDot((c, r) =>
        {
            if (allDotsOnBoard[c, r] == null)
            {
                nullCount++;
            }
            else if (nullCount > 0)
            {
                allDotsOnBoard[c, r].GetComponent<Dot>().row -= nullCount;
                allDotsOnBoard[c, r] = null;
            }
        }, afterEachColumnAction: () =>
        {
            nullCount = 0;
        });
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private void RefillBoard()
    {
        this.doForEveryDot((c, r) =>
        {
            if (allDotsOnBoard[c, r] == null)
            {
                var tempPosition = new Vector2(c, r + offset);
                var dotToUse = Random.Range(0, availableDotColors.Length);
                var gameObject = Instantiate(availableDotColors[dotToUse], tempPosition, Quaternion.identity);
                allDotsOnBoard[c, r] = gameObject;
                gameObject.GetComponent<Dot>().column = c;
                gameObject.GetComponent<Dot>().row = r;
            }
        });
    }

    private bool AreAnyMatchesOnBoard()
    {
        var areAnyMatchesOnBoard = false;
        this.doForEveryDot((c, r) =>
        {
            if (allDotsOnBoard[c, r] != null && allDotsOnBoard[c, r].GetComponent<Dot>().isMatched)
            {
                areAnyMatchesOnBoard = true;
            }
        });
        return areAnyMatchesOnBoard;
    }

    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while (AreAnyMatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyAllMatches();
        }
        currentDot = null;
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.Move;
    }
}
