using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public int x;
    public int y;

    public CheckerColor color;

    public bool isKing = false;

    public Dictionary<Square, Checker> possibleSquares = new Dictionary<Square, Checker>();
    public Square SquareUnderChecker
    {
        get;
        set;
    }

    //private void Start()
    //{
    //    isKing = false;
    //    possibleSquares = new ListDictionary();
    //}

}
