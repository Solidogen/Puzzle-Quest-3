using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                backgroundTile.transform.parent = transform;
                backgroundTile.name = cellName;

                int dotToUse = Random.Range(0, availableDotTypes.Length);
                GameObject dot = Instantiate(availableDotTypes[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = transform;
                dot.name = cellName;

                allDotsOnBoard[i, j] = dot;
            }
        }
    }
}
