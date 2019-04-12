using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float radianAngle = 0;

    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        column = targetX;
        previousColumn = column;
        targetY = (int)transform.position.y;
        row = targetY;
        previousRow = row;
    }

    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        // X
        tempPosition = new Vector2(targetX, transform.position.y);
        var dotAtCoordinates = board.allDotsOnBoard[column, row];
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            // move towards the target
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.3f);
            if (dotAtCoordinates != gameObject)
            {
                dotAtCoordinates = gameObject;
            }
        }
        else
        {
            // directly set the position
            transform.position = tempPosition;
        }
        // Y
        tempPosition = new Vector2(transform.position.x, targetY);
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            // move towards the target
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.3f);
            if (dotAtCoordinates != gameObject)
            {
                dotAtCoordinates = gameObject;
            }
        }
        else
        {
            // directly set the position
            transform.position = tempPosition;
        }
        name = "dot" + $"( {column}, {row} )"; ;
    }

    public IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        if (otherDot != null)
        {
            var otherDotScript = otherDot.GetComponent<Dot>();
            if (!isMatched && !otherDotScript.isMatched)
            {
                otherDotScript.column = column;
                otherDotScript.row = row;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroyAllMatches();
            }
            otherDot = null;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if (firstTouchPosition.Equals(finalTouchPosition))
        {
            return;
        }
        radianAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        CheckAngle();
    }

    private void CheckAngle()
    {
        var swipeAngle = SwipeHelpers.getSwipeAngleFromRadian(radianAngle);
        Debug.Log($"Swipe {swipeAngle}");
        SwapGemIfPossible(swipeAngle);
    }

    private void SwapGemIfPossible(SwipeAngle swipeAngle)
    {
        switch (swipeAngle)
        {
            case SwipeAngle.Right:
                if (column < board.width - 1)
                {
                    otherDot = board.allDotsOnBoard[column + 1, row];
                    otherDot.GetComponent<Dot>().column--;
                    column++;
                }
                break;
            case SwipeAngle.Up:
                if (row < board.height - 1)
                {
                    otherDot = board.allDotsOnBoard[column, row + 1];
                    otherDot.GetComponent<Dot>().row--;
                    row++;
                }
                break;
            case SwipeAngle.Left:
                if (column > 0)
                {
                    otherDot = board.allDotsOnBoard[column - 1, row];
                    otherDot.GetComponent<Dot>().column++;
                    column--;
                }
                break;
            case SwipeAngle.Down:
                if (row > 0)
                {
                    otherDot = board.allDotsOnBoard[column, row - 1];
                    otherDot.GetComponent<Dot>().row++;
                    row--;
                }
                break;
        }
        StartCoroutine(CheckMoveCoroutine());
    }

    private void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDotsOnBoard[column - 1, row];
            GameObject rightDot1 = board.allDotsOnBoard[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == gameObject.tag && rightDot1.tag == gameObject.tag)
                {
                    isMatched = true;
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                }
            }

        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDotsOnBoard[column, row + 1];
            GameObject downDot1 = board.allDotsOnBoard[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == gameObject.tag && downDot1.tag == gameObject.tag)
                {
                    isMatched = true;
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                }
            }
        }
    }
}
