using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        Setup();
    }

    private void Setup() {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var tempPosition = new Vector2(i, j);
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = $"( {i}, {j} )";
            }
        }
    }
}
