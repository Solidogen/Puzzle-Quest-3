using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] availableDotTypes;
    public GameObject[,] allDotsOnBoard;
    private BackgroundTile[,] allBackgroundTiles;

    void Start()
    {
        allBackgroundTiles = new BackgroundTile[width, height];
        allDotsOnBoard = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cellName = $"( {i}, {j} )";

                var tempPosition = new Vector2(i, j);
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.name = "bg";

                int dotToUse = Random.Range(0, availableDotTypes.Length);

                int maxIterations = 0;
                while(MatchesAt(i, j, availableDotTypes[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, availableDotTypes.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }

                GameObject dot = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = transform;
                dot.name = "dot" + cellName;
                backgroundTile.transform.parent = dot.transform;

                allDotsOnBoard[i, j] = dot;
            }
        }
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
        var dot = allDotsOnBoard[column, row];
        if (dot != null && dot.GetComponent<Dot>().isMatched)
        {
            Destroy(dot);
            dot = null;
        }
    }

    public void DestroyAllMatches()
    {
        doForEveryDot((i, j) => {
            DestroyMatchesAt(i, j);
        });
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        yield return new WaitForSeconds(0.4f);
    }

    private void doForEveryDot(Action<int, int> action)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                action.Invoke(i, j);
            }
        }
    }
}
