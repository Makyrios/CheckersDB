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

    public ListDictionary possibleSquares = new ListDictionary();
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
