using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();        
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        board.doForEveryDot((i, j) => {
            GameObject currentDot = board.allDotsOnBoard[i, j];
        });
    }
}
