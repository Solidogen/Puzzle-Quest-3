using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject[] availableDotTypes;
    public GameObject[,] allDotsOnBoard;
    private BackgroundTile[,] allBackgroundTiles;
    public GameObject destroyEffect;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allBackgroundTiles = new BackgroundTile[width, height];
        allDotsOnBoard = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        this.doForEveryDot((i, j) =>
        {
            var cellName = $"( {i}, {j} )";

            var tempPosition = new Vector2(i, j + offset);
            var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
            backgroundTile.name = "bg";

            int dotToUse = Random.Range(0, availableDotTypes.Length);

            int maxIterations = 0;
            while (MatchesAt(i, j, availableDotTypes[dotToUse]) && maxIterations < 100)
            {
                dotToUse = Random.Range(0, availableDotTypes.Length);
                maxIterations++;
                Debug.Log(maxIterations);
            }

            GameObject dot = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
            dot.GetComponent<Dot>().row = j;
            dot.GetComponent<Dot>().column = i;
            dot.transform.parent = transform;
            dot.name = "dot" + cellName;
            backgroundTile.transform.parent = dot.transform;

            allDotsOnBoard[i, j] = dot;
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

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDotsOnBoard[column, row] != null && allDotsOnBoard[column, row].GetComponent<Dot>().isMatched)
        {
            findMatches.currentMatches.Remove(allDotsOnBoard[column, row]);
            GameObject particle = Instantiate(destroyEffect, allDotsOnBoard[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDotsOnBoard[column, row]);
            allDotsOnBoard[column, row] = null;
        }
    }

    public void DestroyAllMatches()
    {
        this.doForEveryDot((i, j) =>
        {
            DestroyMatchesAt(i, j);
        });
        StartCoroutine(DecreaseRowCoroutine());
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        this.doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] == null)
            {
                nullCount++;
            }
            else if (nullCount > 0)
            {
                allDotsOnBoard[i, j].GetComponent<Dot>().row -= nullCount;
                allDotsOnBoard[i, j] = null;
            }
        }, afterEachColumnAction: () => {
            nullCount = 0;
        });
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private void RefillBoard()
    {
        this.doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] == null)
            {
                var tempPosition = new Vector2(i, j + offset);
                var dotToUse = Random.Range(0, availableDotTypes.Length);
                var gameObject = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
                allDotsOnBoard[i, j] = gameObject;
                gameObject.GetComponent<Dot>().column = i;
                gameObject.GetComponent<Dot>().row = j;
            }
        });
    }

    private bool AreAnyMatchesOnBoard()
    {
        var areAnyMatchesOnBoard = false;
        this.doForEveryDot((i, j) => {
            if (allDotsOnBoard[i, j] != null && allDotsOnBoard[i, j].GetComponent<Dot>().isMatched)
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
        while(AreAnyMatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyAllMatches();
        }
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.Move;
    }
}
