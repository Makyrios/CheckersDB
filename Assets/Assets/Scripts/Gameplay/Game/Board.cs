using System.Collections.Generic;

public class Board
{
    public Checker[,] checkers;
    public List<Checker> whiteCheckers;
    public List<Checker> blackCheckers;
    public List<Square> squares;
    public List<Checker> possibleCheckers;

    public Board()
    {
        checkers = new Checker[8, 8];
        whiteCheckers = new List<Checker>();
        blackCheckers = new List<Checker>();
        squares = new List<Square>();
        possibleCheckers = new List<Checker>();
    }


}
