using System.Collections.Generic;
using System.Linq;

public class Board
{
    public Checker[,] checkers;
    public List<Checker> whiteCheckers;
    public List<Checker> blackCheckers;
    public List<Square> squares;
    public static bool isWhiteTurn = MoveCheckers.isWhiteTurn;
    public List<Checker> possibleCheckers;
    public bool isStreak;

    public Board()
    {
        checkers = new Checker[8, 8];
        whiteCheckers = new List<Checker>();
        blackCheckers = new List<Checker>();
        squares = new List<Square>();
        possibleCheckers = new List<Checker>();
        isStreak = false;
    }
    public Board(Checker[,] board, List<Checker> white, List<Checker> black, List<Square> moves)
    {
        checkers = board.Clone() as Checker[,];
        whiteCheckers = white.Select(s => new Checker(s)).ToList();
        blackCheckers = black.Select(s => new Checker(s)).ToList();
        squares = moves.Select(s => new Square(s)).ToList();
        possibleCheckers = new List<Checker>();
    }


    public List<KeyValuePair<Checker, Square>> GetMoves()
    {
        List<KeyValuePair<Checker, Square>> moves = new List<KeyValuePair<Checker, Square>>();
        var checkers = MoveCheckers.GetAllPossibleCheckers(this);
        foreach (Checker checker in checkers)
        {
            foreach (Square move in checker.possibleSquares)
            {
                moves.Add(new KeyValuePair<Checker, Square>(checker, move));
            }
        }

        return moves;
    }

    public Board MakeMove(KeyValuePair<Checker, Square> move)
    {
        Checker checker = move.Key;
        Square square = move.Value;
        Checker checkerToRemove = MoveCheckers.FindCheckerToRemove(this, checker, square);
        Board newBoard = new Board(checkers, whiteCheckers, blackCheckers, squares);

        int checker_i = checker.y;
        int checker_j = checker.x;
        int square_i = square.y;
        int square_j = square.x;

        // Change square value
        newBoard.checkers[checker_i, checker_j] = null;
        // Remove checker from previous square
        newBoard.checkers[square_i, square_j] = checker;
        if (checkerToRemove != null)
        {
            MoveCheckers.DestroyChecker(newBoard, checkerToRemove);
            //newBoard.checkers[checkerToRemove.y, checkerToRemove.x] = null;
        }

        return newBoard;
    }

}