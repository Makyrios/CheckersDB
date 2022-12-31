using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCheckers : MonoBehaviour
{
    #region Objects
    [SerializeField]
    GameObject EndTurnButton;
    GameOver gameOver;
    private static Board currentBoard;
    #endregion

    #region MoveVariables

    private RaycastHit2D mouseOverHit;
    private Vector2 ClickedPosition;
    private Checker selectedChecker;
    private Square selectedSquare;

    public static bool isWhiteTurn;
    private static bool isStreak;
    private bool isFirstFrame;

    private CurrentTurn turnText;

    #endregion



    // Start is called before the first frame update
    void Start()
    {
        currentBoard = PaintBoard.currentBoard;

        EndTurnButton.SetActive(false);
        isFirstFrame = true;
        isWhiteTurn = true;
        turnText = FindObjectOfType<CurrentTurn>();
        gameOver = FindObjectOfType<GameOver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstFrame)
        {
            HightlightPossibleCheckers(currentBoard);
            isFirstFrame = false;
        }

        TrySelectObject();
        TryMoveChecker(selectedChecker, selectedSquare);
    }

    /// <summary>
    /// Trying to:
    /// -Select checker piece if not selected
    /// -Select square if checker piece is selected
    /// -Press on an UI button
    /// </summary>
    private void TrySelectObject()
    {
        // Select checker on click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            UpdateMouseOver();
            if (mouseOverHit.collider == null)
                return;

            if (mouseOverHit.collider.CompareTag("Checker"))
            {
                if (currentBoard.possibleCheckers.Count > 0 && currentBoard.possibleCheckers.Contains(mouseOverHit.collider.gameObject.GetComponent<Checker>()) ||
                    currentBoard.possibleCheckers.Count == 0)
                {
                    ReturnSquaresColor(selectedChecker);
                    SelectChecker(mouseOverHit);
                    AddPossibleMoves(currentBoard, selectedChecker);
                    HighlightPossibleSquares(selectedChecker);
                }
            }
            else if (mouseOverHit.collider.CompareTag("Square"))
            {
                SelectSquare(mouseOverHit);
            }
        }
    }

    /// <summary>
    /// Returns color of highlighted squares, moves the checker if possible,
    /// checks if selectedChecker became a king, keeps a track of streak moves
    /// </summary>
    private void TryMoveChecker(Checker checker, Square square)
    {
        if (Input.GetMouseButton(0))
        {
            UpdateMouseOver();
            if (!IsInsideBoard(mouseOverHit.point))
                return;

            if (square == null || checker == null)
                return;

            if (checker.possibleSquares.Contains(square))
            {
                ReturnSquaresColor(checker);
                // Returns color of square under selected checker
                checker.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.black;

                // If selected checker jumps over an enemy, then save it
                Checker checkerToRemove = FindCheckerToRemove(currentBoard, checker, square);
                MoveChecker(currentBoard, checker, square, checkerToRemove);

                if (CheckIfBecomeKing(checker))
                {
                    EndTurn();
                    return;
                }

                CheckMultipleTurn(currentBoard, checker, checkerToRemove);

            }
        }
    }

    private void CheckMultipleTurn(Board board, Checker checker, Checker checkerToRemove)
    {
        // If we can destroy enemy in a row do not end turn
        if (checkerToRemove != null)
        {
            checker.possibleSquares.Clear();
            isStreak = true;
            AddPossibleMoves(board, checker);
            isStreak = false;
            if (HasBeatingMove(board, checker))
            {
                isStreak = true;
            }
            if (isStreak)
            {
                EndMove();
            }
            else
            {
                EndTurn();
            }
        }
        else
        {
            EndTurn();
        }
    }

    private bool CheckIfBecomeKing(Checker checker)
    {
        // Check if moved checker became king
        if (!checker.isKing && checker.color == CheckerColor.White && checker.y == 0 ||
            checker.color == CheckerColor.Black && checker.y == 7)
        {
            checker.isKing = true;
            if (checker.color == CheckerColor.White)
            {
                checker.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WhiteCheckerCrownSprite");
            }
            else
            {
                checker.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BlackCheckerCrownSprite");
            }
            return true;
        }
        return false;
    }

    public static Checker FindCheckerToRemove(Board board, Checker checker, Square square)
    {
        if (checker == null || square == null)
        {
            return null;
        }
        if (!checker.isKing)
        {
            if (Mathf.Abs(checker.y - square.y) == 2)
            {
                if (checker.color == CheckerColor.White)
                {
                    return board.checkers[checker.y - 1, (checker.x + square.x) / 2];
                }
                else
                {
                    return board.checkers[checker.y + 1, (checker.x + square.x) / 2];
                }
            }
        }
        else
        {
            int i = checker.y;
            int j = checker.x;
            // Move along square
            do
            {
                i = (i <= square.y) ? ++i : --i;
                j = (j <= square.x) ? ++j : --j;
            } while (board.checkers[i, j] == null && (i != square.y && j != square.x));

            return board.checkers[i, j];
        }
        return null;
    }

    /// <summary>
    /// Get position of mouse
    /// </summary>
    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Camera was not found");
            return;
        }
        mouseOverHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    private bool IsInsideBoard(Vector2 pos)
    {
        return (pos.x >= PaintBoard.StartPosition.x && pos.x <= PaintBoard.EndPosition.x
            && pos.y <= PaintBoard.StartPosition.y && pos.y >= PaintBoard.EndPosition.y);
    }

    /// <summary>
    /// Appropriately select checker of current turn
    /// </summary>
    /// <param name="hit"></param>
    private void SelectChecker(RaycastHit2D hit)
    {
        Checker checker = hit.collider.GetComponent<Checker>();
        if (checker != null)
        {
            if (checker.color == CheckerColor.White && isWhiteTurn ||
                checker.color == CheckerColor.Black && !isWhiteTurn)
            {
                if (selectedChecker != checker)
                {
                    selectedChecker = checker;
                }
                else
                {
                    selectedChecker = null;
                }
            }
        }
    }

    // Move destroying enemy upper left
    private static void MoveDestroyingUpperLeft(Board board, Checker checker)
    {
        if (checker.x > 1 && checker.y > 1 &&
            board.checkers[checker.y - 1, checker.x - 1] != null &&
            board.checkers[checker.y - 1, checker.x - 1].color == CheckerColor.Black &&
            board.checkers[checker.y - 2, checker.x - 2] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y - 2 && x.x == checker.x - 2));
        }
    }

    // Move destroying enemy upper left
    private static void MoveDestroyingUpperRight(Board board, Checker checker)
    {
        if (checker.x < 6 && checker.y > 1 &&
            board.checkers[checker.y - 1, checker.x + 1] != null &&
            board.checkers[checker.y - 1, checker.x + 1].color == CheckerColor.Black &&
            board.checkers[checker.y - 2, checker.x + 2] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y - 2 && x.x == checker.x + 2));
        }
    }

    // Move destroying enemy lower left
    private static void MoveDestroyingLowerLeft(Board board, Checker checker)
    {
        if (checker.x > 1 && checker.y < 6 &&
            board.checkers[checker.y + 1, checker.x - 1] != null &&
            board.checkers[checker.y + 1, checker.x - 1].color == CheckerColor.White &&
            board.checkers[checker.y + 2, checker.x - 2] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y + 2 && x.x == checker.x - 2));
        }
    }

    // Move destroying enemy lower right
    private static void MoveDestroyingLowerRight(Board board, Checker checker)
    {
        if (checker.x < 6 && checker.y < 6 &&
            board.checkers[checker.y + 1, checker.x + 1] != null &&
            board.checkers[checker.y + 1, checker.x + 1].color == CheckerColor.White &&
            board.checkers[checker.y + 2, checker.x + 2] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y + 2 && x.x == checker.x + 2));
        }
    }

    private static void MoveLowerRight(Board board, Checker checker)
    {
        // Move lower right
        if (checker.x < 7 &&
            board.checkers[checker.y + 1, checker.x + 1] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y + 1 && x.x == checker.x + 1));
        }
    }

    private static void MoveLowerLeft(Board board, Checker checker)
    {
        // Move lower left
        if (checker.x > 0 &&
        board.checkers[checker.y + 1, checker.x - 1] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y + 1 && x.x == checker.x - 1));
        }
    }

    private static void MoveUpperRight(Board board, Checker checker)
    {
        // Move upper right
        if (checker.x < 7 &&
            board.checkers[checker.y - 1, checker.x + 1] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y - 1 && x.x == checker.x + 1));
        }
    }

    private static void MoveUpperLeft(Board board, Checker checker)
    {
        // Move upper left
        if (checker.x > 0 &&
            board.checkers[checker.y - 1, checker.x - 1] == null)
        {
            checker.possibleSquares.Add(board.squares.Find(x => x.y == checker.y - 1 && x.x == checker.x - 1));
        }
    }

    /// <summary>
    /// Adding squares, that are allowed to a checker
    /// </summary>
    /// <param name="checker">Checking moves for this checker</param>
    /// <param name="isAddingPossibleCheckers">Needed to fill possibleCheckers list</param>
    public static void AddPossibleMoves(Board board, Checker checker, bool isAddingPossibleCheckers = false)
    {
        if (checker == null)
            return;
        checker.possibleSquares.Clear();

        // Add all possible move squares to list
        #region DefaultCheckerMovement
        if (!checker.isKing)
        {
            if (checker.color == CheckerColor.White)
            {
                if (checker.y > 0)
                {
                    if (!isAddingPossibleCheckers)
                    {
                        // If possibleCheckers contains current checker then 
                        // add only destroying moves
                        if (board.possibleCheckers.Count > 0)
                        {
                            if (board.possibleCheckers.Contains(checker))
                            {
                                MoveDestroyingUpperLeft(board, checker);
                                MoveDestroyingUpperRight(board, checker);
                                return;
                            }
                            else
                                return;
                        }
                    }

                    // If on streak then we can move forward destroying enemy checkers
                    if (isStreak)
                    {
                        MoveDestroyingUpperLeft(board, checker);
                        MoveDestroyingUpperRight(board, checker);
                    }
                    else
                    {
                        MoveUpperLeft(board, checker);
                        MoveUpperRight(board, checker);
                        MoveDestroyingUpperLeft(board, checker);
                        MoveDestroyingUpperRight(board, checker);
                    }

                }
            }
            // If selected checker is black
            else
            {
                if (checker.y < 7)
                {
                    // If possibleCheckers contains current checker then 
                    // add only destroying moves
                    if (!isAddingPossibleCheckers)
                    {
                        if (board.possibleCheckers.Count > 0)
                        {
                            if (board.possibleCheckers.Contains(checker))
                            {
                                MoveDestroyingLowerLeft(board, checker);
                                MoveDestroyingLowerRight(board, checker);
                                return;
                            }
                            else
                                return;
                        }
                    }

                    // If on streak then we can move forward destroying enemy checker
                    if (isStreak)
                    {
                        MoveDestroyingLowerLeft(board, checker);
                        MoveDestroyingLowerRight(board, checker);
                    }
                    else
                    {
                        MoveLowerLeft(board, checker);
                        MoveLowerRight(board, checker);
                        MoveDestroyingLowerLeft(board, checker);
                        MoveDestroyingLowerRight(board, checker);
                    }
                }
            }
        }
        #endregion
        // If selected checker is a king
        #region KingMovement
        else
        {
            AddKingPossibleMoves(board, checker);
        }
        #endregion
    }

    /// <summary>
    /// King moves at all diagonal directions without restrictions
    /// </summary>
    /// <param name="checker">Adds movement for this checker</param>
    /// <param name="isAddingPossibleCheckers">Needed to fill possibleCheckers list</param>
    private static void AddKingPossibleMoves(Board board, Checker checker, bool isAddingPossibleCheckers = false)
    {
        if (!checker.isKing)
            return;

        CheckerColor color = checker.color;
        CheckerColor oppositeColor = (color == CheckerColor.White) ? CheckerColor.Black : CheckerColor.White;
        bool isDestroying = false;
        // Diagonal left upper move
        int i = checker.y - 1;
        int j = checker.x - 1;

        while (i >= 0 && j >= 0)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (board.checkers[i, j] != null && isDestroying)
            {
                break;
            }
            // If current square is empty
            if (board.checkers[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
                else
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
            }
            // If we meet enemy checker then mark it and go further
            if (board.checkers[i, j] != null && board.checkers[i, j].color == oppositeColor)
            {
                if (i - 1 >= 0 && j - 1 >= 0 && board.checkers[i - 1, j - 1] == null)
                {
                    isDestroying = true;
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (board.checkers[i, j] != null && board.checkers[i, j].color == color)
            {
                break;
            }

            i--;
            j--;
        }

        // Diagonal right upper move
        isDestroying = false;
        i = checker.y - 1;
        j = checker.x + 1;
        while (i >= 0 && j <= 7)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (board.checkers[i, j] != null && isDestroying)
            {
                break;
            }

            if (board.checkers[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
                else
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
            }
            // If we meet enemy checker then mark it and go further
            if (board.checkers[i, j] != null && board.checkers[i, j].color == oppositeColor)
            {
                if (i - 1 >= 0 && j + 1 <= 7 && board.checkers[i - 1, j + 1] == null)
                {
                    isDestroying = true;
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (board.checkers[i, j] != null && board.checkers[i, j].color == color)
            {
                break;
            }

            i--;
            j++;
        }

        // Diagonal left lower move
        isDestroying = false;
        i = checker.y + 1;
        j = checker.x - 1;
        while (i <= 7 && j >= 0)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (board.checkers[i, j] != null && isDestroying)
            {
                break;
            }

            if (board.checkers[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
                else
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
            }
            // If we meet enemy checker then mark it and go further
            if (board.checkers[i, j] != null && board.checkers[i, j].color == oppositeColor)
            {
                if (i + 1 <= 7 && j - 1 >= 0 && board.checkers[i + 1, j - 1] == null)
                {
                    isDestroying = true;
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (board.checkers[i, j] != null && board.checkers[i, j].color == color)
            {
                break;
            }

            i++;
            j--;
        }

        // Diagonal right lower move
        isDestroying = false;
        i = checker.y + 1;
        j = checker.x + 1;
        while (i <= 7 && j <= 7)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (board.checkers[i, j] != null && isDestroying)
            {
                break;
            }

            if (board.checkers[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
                else
                    checker.possibleSquares.Add(board.squares.Find(x => x.y == i && x.x == j));
            }
            // If we meet enemy checker then mark it and go further
            if (board.checkers[i, j] != null && board.checkers[i, j].color == oppositeColor)
            {
                if (i + 1 <= 7 && j + 1 <= 7 && board.checkers[i + 1, j + 1] == null)
                {
                    isDestroying = true;
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (board.checkers[i, j] != null && board.checkers[i, j].color == color)
            {
                break;
            }

            i++;
            j++;
        }

        // Remain only destroying moves
        if (!isAddingPossibleCheckers)
        {
            if (board.possibleCheckers.Count > 0)
            {
                if (board.possibleCheckers.Contains(checker))
                {
                    List<Square> squares = new List<Square>();
                    foreach (Square sq in checker.possibleSquares)
                    {
                        Checker checkerToRemove = FindCheckerToRemove(board, checker, sq);
                        if (checkerToRemove != null)
                        {
                            squares.Add(sq);
                        }
                    }
                    checker.possibleSquares = squares;
                }
            }
        }
    }

    /// <summary>
    /// Return list of all possibles checkers for current turn
    /// </summary>
    /// <returns>List of all possible checkers for current turn</returns>
    public static List<Checker> GetAllPossibleCheckers(Board board)
    {
        List<Checker> possibleMoves = new List<Checker>();

        // If we can beat enemy, then add only beating moves to the list
        FindBeatingCheckers(board);
        if (board.possibleCheckers.Count > 0)
        {
            foreach (Checker checker in board.possibleCheckers)
            {
                possibleMoves.Add(checker);
            }
            return possibleMoves;
        }

        if (isWhiteTurn)
        {
            foreach (Checker checker in board.whiteCheckers)
            {
                if (checker.possibleSquares.Count > 0)
                {
                    possibleMoves.Add(checker);
                }
            }
        }
        // Find possible moves for black checkers
        else
        {
            foreach (Checker checker in board.blackCheckers)
            {
                if (checker.possibleSquares.Count > 0)
                {
                    possibleMoves.Add(checker);
                }
            }
        }
        return possibleMoves;


    }

    private void HighlightPossibleSquares(Checker checker)
    {
        if (checker != null)
        {
            foreach (Square square in checker.possibleSquares)
            {
                var sq = square;
                var renderer = sq.gameObject.GetComponent<SpriteRenderer>();
                renderer.color = Color.yellow;
            }
        }
    }

    /// <summary>
    /// Highlights possible checkers to select. If any checker can beat enemy, than they are only option to select
    /// </summary>
    private void HightlightPossibleCheckers(Board board)
    {
        List<Checker> possibleMoves = GetAllPossibleCheckers(board);
        foreach (Checker checker in possibleMoves)
        {
            checker.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void ReturnSquaresColor(Checker checker)
    {
        if (checker == null) return;
        foreach (Square square in checker.possibleSquares)
        {
            var sq = square;
            var renderer = sq.gameObject.GetComponent<SpriteRenderer>();
            renderer.color = Color.black;
        }
    }

    private void ReturnCheckersColor()
    {
        // Before ending turn return checkers to normal color
        if (isWhiteTurn)
        {
            foreach (Checker ch in currentBoard.whiteCheckers)
            {
                ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            foreach (Checker ch in currentBoard.blackCheckers)
            {
                ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    private void SelectSquare(RaycastHit2D hit)
    {
        Square square = hit.collider.GetComponent<Square>();
        if (square != null && square.GetComponent<SpriteRenderer>().color != Color.white && selectedChecker != null)
        {
            selectedSquare = square;
        }
    }

    /// <summary>
    /// Moves checker to selected square
    /// </summary>
    /// <param name="checker">Checker to move</param>
    /// <param name="square">Selected square to move</param>
    /// <param name="checkerToRemove">Used for beating enemy checker if jumping over it</param>
    public static void MoveChecker(Board board, Checker checker, Square square, Checker checkerToRemove = null)
    {
        // Move checker to selected square
        checker.transform.position = ExtensionMethods.GetVectorPosition(square.x, square.y);
        var zc = checker.transform.position;
        zc.z = -1f;
        checker.transform.position = zc;
        // If jump over the enemy checker then kill
        if (checkerToRemove != null)
        {
            DestroyChecker(board, checkerToRemove);
        }
        // Change status of squares
        board.checkers[checker.y, checker.x] = null;
        board.checkers[square.y, square.x] = checker;
        checker.x = square.x;
        checker.y = square.y;
        checker.SquareUnderChecker = square;
    }

    public static void DestroyChecker(Board board, Checker checker)
    {
        if (checker != null)
        {
            board.checkers[checker.y, checker.x] = null;
            if (checker.color == CheckerColor.White)
            {
                board.whiteCheckers.Remove(checker);
            }
            else
            {
                board.blackCheckers.Remove(checker);
            }
            Destroy(checker.gameObject);
        }
    }

    /// <summary>
    /// Iterates over all checkers of current turn to check if any of them can beat enemy.
    /// If there is, than add this checker to possibleCheckers list
    /// </summary>
    private static void FindBeatingCheckers(Board board)
    {
        board.possibleCheckers.Clear();
        if (isWhiteTurn)
        {
            foreach (Checker ch in board.whiteCheckers)
            {
                AddPossibleMoves(board, ch, true);
                if (HasBeatingMove(board, ch))
                {
                    board.possibleCheckers.Add(ch);
                }
            }
        }
        else
        {
            foreach (Checker ch in board.blackCheckers)
            {
                AddPossibleMoves(board, ch, true);
                if (HasBeatingMove(board, ch))
                {
                    board.possibleCheckers.Add(ch);
                }
            }
        }
    }

    private static bool HasBeatingMove(Board board, Checker ch)
    {
        foreach (Square sq in ch.possibleSquares)
        {
            Checker checkerToBeat = FindCheckerToRemove(board, ch, sq);
            if (checkerToBeat != null)
            {
                // If there at least 1 checker we can beat, then add it to list
                return true;
            }
        }
        return false;
    }

    private void HandleStreakChecker(Board board, Checker checker)
    {
        if (!isStreak)
        {
            return;
        }
        board.possibleCheckers.Clear();
        board.possibleCheckers.Add(checker);
        checker.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    /// <summary>
    /// Called during a streak. You can end turn on any streak move
    /// </summary>
    private void EndMove()
    {
        EndTurnButton.SetActive(true);
        CheckVictory(currentBoard);
        ReturnCheckersColor();
        HandleStreakChecker(currentBoard, selectedChecker);
        selectedChecker = null;
        selectedSquare = null;
    }

    public void EndTurn()
    {
        EndTurnButton.SetActive(false);
        isStreak = false;
        CheckVictory(currentBoard);
        ReturnCheckersColor();
        selectedChecker = null;
        selectedSquare = null;
        isWhiteTurn = !isWhiteTurn;
        turnText.ChangeTurn();
        HightlightPossibleCheckers(currentBoard);

    }

    private void CheckVictory(Board board)
    {
        if (board.whiteCheckers.Count == 0)
        {
            gameOver.HandleOnGameOverEvent(CheckerColor.Black);
        }
        else if (board.blackCheckers.Count == 0)
        {
            gameOver.HandleOnGameOverEvent(CheckerColor.White);
        }
    }


}
