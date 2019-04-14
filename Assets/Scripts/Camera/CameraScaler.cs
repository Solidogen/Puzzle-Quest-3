using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffsetZ;
    public float aspectRatio = 10 / 16f;
    public float padding = 2;

    void Start()
    {
        board = FindObjectOfType<Board>();
        board?.Also(b =>
        {
            RepositionCamera(b.width - 1, b.height - 1);
        });
    }

    void RepositionCamera(float x, float y)
    {
        var tempPosition = new Vector3(x / 2, y / 2, cameraOffsetZ);
        transform.position = tempPosition;
        Camera.main.orthographicSize = (board.width >= board.height)
            ? (board.width / 2 + padding) / aspectRatio
            : board.height / 2 + padding;
    }
}
