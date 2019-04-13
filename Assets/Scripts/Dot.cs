using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
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

    [Header("Swipe Variables")]
    public float radianAngle = 0;

    [Header("Powerup Variables")]
    public DotType dotType = DotType.Normal;
    public GameObject columnBomb;
    public GameObject rowBomb;
    public GameObject colorBomb;
    public GameObject adjacentBomb;

    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    void Update()
    {
        // if (isMatched)
        // {
        //     SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        //     sprite.color = new Color(1f, 1f, 1f, .2f);
        // }
        targetX = column;
        targetY = row;
        // X
        tempPosition = new Vector2(targetX, transform.position.y);
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            // move towards the target
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.3f);
            if (board.allDotsOnBoard[column, row] != gameObject)
            {
                board.allDotsOnBoard[column, row] = gameObject;
            }
            findMatches.FindAllMatches();
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
            if (board.allDotsOnBoard[column, row] != gameObject)
            {
                board.allDotsOnBoard[column, row] = gameObject;
            }
            findMatches.FindAllMatches();
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
        if (dotType == DotType.ColorBomb)
        {
            //this dot is a color bomb, the other piece is the color to destroy
            findMatches.MatchDotsOfColor(otherDot.tag);
            isMatched = true;
        }
        else if (otherDot.GetComponent<Dot>().dotType == DotType.ColorBomb)
        {
            //other dot is a color bomb, this dot is the color to destroy
            findMatches.MatchDotsOfColor(gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherDot != null)
        {
            var otherDotScript = otherDot.GetComponent<Dot>();
            if (!isMatched && !otherDotScript.isMatched)
            {
                otherDotScript.column = column;
                otherDotScript.row = row;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = GameState.Move;
            }
            else
            {
                board.DestroyAllMatches();
            }
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.Wait)
        {
            return;
        }
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.Wait)
        {
            return;
        }
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if (firstTouchPosition.Equals(finalTouchPosition))
        {
            board.currentState = GameState.Move;
            return;
        }
        radianAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        CheckAngle();
        board.currentState = GameState.Wait;
        board.currentDot = this;
    }

    private void CheckAngle()
    {
        SwipeHelpers.getSwipeAngleFromRadian(radianAngle).Also(angle =>
        {
            SwapGemIfPossible(angle);
        });
    }

    private void SwapGemIfPossible(SwipeAngle swipeAngle)
    {
        switch (swipeAngle)
        {
            case SwipeAngle.Right:
                if (column < board.width - 1)
                {
                    otherDot = board.allDotsOnBoard[column + 1, row];
                    previousRow = row;
                    previousColumn = column;
                    otherDot.GetComponent<Dot>().column--;
                    column++;
                }
                break;
            case SwipeAngle.Up:
                if (row < board.height - 1)
                {
                    otherDot = board.allDotsOnBoard[column, row + 1];
                    previousRow = row;
                    previousColumn = column;
                    otherDot.GetComponent<Dot>().row--;
                    row++;
                }
                break;
            case SwipeAngle.Left:
                if (column > 0)
                {
                    otherDot = board.allDotsOnBoard[column - 1, row];
                    previousRow = row;
                    previousColumn = column;
                    otherDot.GetComponent<Dot>().column++;
                    column--;
                }
                break;
            case SwipeAngle.Down:
                if (row > 0)
                {
                    otherDot = board.allDotsOnBoard[column, row - 1];
                    previousRow = row;
                    previousColumn = column;
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

    public void MakeBomb(DotType type)
    {
        if (dotType == type)
        {
            return;
        }
        isMatched = false;
        dotType = type;
        GameObject bombToInstantiate = null;
        switch (dotType)
        {
            case DotType.ColumnBomb:
                bombToInstantiate = columnBomb;
                break;
            case DotType.RowBomb:
                bombToInstantiate = rowBomb;
                break;
            case DotType.ColorBomb:
                bombToInstantiate = colorBomb;
                break;
            case DotType.AdjacentBomb:
                bombToInstantiate = adjacentBomb;
                break;
            default:
                throw new InvalidOperationException("Bomb type not handled");
        }
        Instantiate(bombToInstantiate, transform.position, Quaternion.identity).Also(arrow =>
        {
            arrow.transform.parent = transform;
        });
    }

    // debug
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dotType = DotType.AdjacentBomb;
            Instantiate(adjacentBomb, transform.position, Quaternion.identity).Also(bomb =>
            {
                bomb.transform.parent = transform;
            });
        }
    }
}
