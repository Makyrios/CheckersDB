using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public int x;
    public int y;

    public CheckerColor color;

    public bool isKing = false;

    public List<Square> possibleSquares = new List<Square>();
    public Square SquareUnderChecker
    {
        get;
        set;
    }

    public Checker(Checker ch)
    {
        this.x = ch.x;
        this.y = ch.y;
        this.color = ch.color;
        this.isKing = ch.isKing;
    }

}
