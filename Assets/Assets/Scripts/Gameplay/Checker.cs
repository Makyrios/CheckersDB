using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public int x;
    public int y;

    public CheckerColor color;

    public bool isKing = false;

    public ListDictionary possibleSquares;
    public Square SquareUnderChecker
    {
        get;
        set;
    }

    private void Start()
    {
        possibleSquares = new ListDictionary();
    }

}