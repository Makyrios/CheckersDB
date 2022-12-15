using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCheckers : MonoBehaviour
{
    [SerializeField]
    GameObject EndTurnButton;

    #region MoveVariables

    private RaycastHit2D mouseOverHit;
    private Vector2 ClickedPosition;
    private Checker selectedChecker;
    private Square selectedSquare;

    public bool isWhiteTurn;
    private bool isStreak;
    private bool isFirstFrame;

    private CurrentTurn turnText;

    // If there is at least one checker that can beat enemy, then add it to possibleCheckers
    // if possibleCheckers is empty then any checker can move
    private List<Checker> possibleCheckers;


    #endregion

    GameOver gameOver;

    // Start is called before the first frame update
    void Start()
    {
        EndTurnButton.SetActive(false);
        isFirstFrame = true;
        isStreak = false;
        isWhiteTurn = true;
        possibleCheckers = new List<Checker>();
        turnText = FindObjectOfType<CurrentTurn>();
        gameOver = FindObjectOfType<GameOver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstFrame)
        {
            HightlightPossibleCheckers();
            isFirstFrame = false;
        }
        TrySelectObject();
        TryMoveChecker();
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
            if (mouseOverHit.collider.CompareTag("Checker"))
            {
                if (possibleCheckers.Count > 0 && possibleCheckers.Contains(mouseOverHit.collider.gameObject.GetComponent<Checker>()) ||
                    possibleCheckers.Count == 0)
                {
                    ReturnSquaresColor(selectedChecker);
                    SelectChecker(mouseOverHit);
                    AddPossibleMoves(selectedChecker);
                    HighlightPossibleSquares();
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
    private void TryMoveChecker()
    {
        // Move selected checker
        if (selectedChecker != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateMouseOver();
                if (!IsInsideBoard(mouseOverHit.point))
                    return;

                if (selectedSquare == null)
                    return;

                if (selectedChecker.possibleSquares.Contains(selectedSquare))
                {
                    ReturnSquaresColor(selectedChecker);

                    // If selected checker jumps over an enemy, then save it
                    Checker checkerToRemove = null;
                    checkerToRemove = FindCheckerToRemove(checkerToRemove);

                    // Returns color of square under selected checker
                    selectedChecker.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.black;

                    MoveChecker(selectedChecker, selectedSquare, checkerToRemove);

                    if (CheckIfBecomeKing())
                    {
                        EndTurn();
                        return;
                    }

                    CheckMultipleTurn(checkerToRemove);

                }
            }
        }
    }

    private void CheckMultipleTurn(Checker checkerToRemove)
    {
        // If we can destroy enemy in a row do not end turn
        if (checkerToRemove != null)
        {
            selectedChecker.possibleSquares.Clear();
            isStreak = true;
            AddPossibleMoves(selectedChecker);
            isStreak = false;
            foreach (DictionaryEntry de in selectedChecker.possibleSquares)
            {
                if (de.Value != null)
                {
                    isStreak = true;
                }
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

    private bool CheckIfBecomeKing()
    {
        // Check if moved checker became king
        if (!selectedChecker.isKing && selectedChecker.color == CheckerColor.White && selectedChecker.y == 0 ||
            selectedChecker.color == CheckerColor.Black && selectedChecker.y == 7)
        {
            selectedChecker.isKing = true;
            if (selectedChecker.color == CheckerColor.White)
            {
                selectedChecker.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WhiteCheckerCrownSprite");
            }
            else
            {
                selectedChecker.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BlackCheckerCrownSprite");
            }
            return true;
        }
        return false;
    }

    private Checker FindCheckerToRemove(Checker checkerToRemove)
    {
        foreach (DictionaryEntry de in selectedChecker.possibleSquares)
        {
            if ((Square)de.Key == selectedSquare && de.Value != null)
            {
                checkerToRemove = (Checker)de.Value;
            }
        }

        return checkerToRemove;
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
    void MoveDestroyingUpperLeft(Checker checker)
    {
        if (checker.x > 1 && checker.y > 1 &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x - 1] != null &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x - 1].color == CheckerColor.Black &&
            PaintBoard.checkersMatrix[checker.y - 2, checker.x - 2] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y - 2 && x.x == checker.x - 2),
                PaintBoard.checkersMatrix[checker.y - 1, checker.x - 1]);
        }
    }

    // Move destroying enemy upper left
    void MoveDestroyingUpperRight(Checker checker)
    {
        if (checker.x < 6 && checker.y > 1 &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x + 1] != null &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x + 1].color == CheckerColor.Black &&
            PaintBoard.checkersMatrix[checker.y - 2, checker.x + 2] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y - 2 && x.x == checker.x + 2),
                PaintBoard.checkersMatrix[checker.y - 1, checker.x + 1]);
        }
    }

    // Move destroying enemy lower left
    void MoveDestroyingLowerLeft(Checker checker)
    {
        if (checker.x > 1 && checker.y < 6 &&
            PaintBoard.checkersMatrix[checker.y + 1, checker.x - 1] != null &&
            PaintBoard.checkersMatrix[checker.y + 1, checker.x - 1].color == CheckerColor.White &&
            PaintBoard.checkersMatrix[checker.y + 2, checker.x - 2] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y + 2 && x.x == checker.x - 2),
                PaintBoard.checkersMatrix[checker.y + 1, checker.x - 1]);
        }
    }

    // Move destroying enemy lower right
    void MoveDestroyingLowerRight(Checker checker)
    {
        if (checker.x < 6 && checker.y < 6 &&
            PaintBoard.checkersMatrix[checker.y + 1, checker.x + 1] != null &&
            PaintBoard.checkersMatrix[checker.y + 1, checker.x + 1].color == CheckerColor.White &&
            PaintBoard.checkersMatrix[checker.y + 2, checker.x + 2] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y + 2 && x.x == checker.x + 2),
                PaintBoard.checkersMatrix[checker.y + 1, checker.x + 1]);
        }
    }

    /// <summary>
    /// Adding sqaures, that are allowed to a checker
    /// </summary>
    /// <param name="checker">Checking moves for this checker</param>
    /// <param name="isAddingPossibleCheckers">Needed to fill possibleCheckers list</param>
    private void AddPossibleMoves(Checker checker, bool isAddingPossibleCheckers = false)
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
                        if (possibleCheckers.Count > 0)
                        {
                            if (possibleCheckers.Contains(checker))
                            {
                                MoveDestroyingUpperLeft(checker);
                                MoveDestroyingUpperRight(checker);
                                return;
                            }
                            else
                                return;
                        }
                    }

                    // If on streak then we can move forward destroying enemy checkers
                    if (isStreak)
                    {
                        MoveDestroyingUpperLeft(checker);
                        MoveDestroyingUpperRight(checker);
                    }
                    else
                    {
                        MoveUpperLeft(checker);
                        MoveUpperRight(checker);
                        MoveDestroyingUpperLeft(checker);
                        MoveDestroyingUpperRight(checker);
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
                        if (possibleCheckers.Count > 0)
                        {
                            if (possibleCheckers.Contains(checker))
                            {
                                MoveDestroyingLowerLeft(checker);
                                MoveDestroyingLowerRight(checker);
                                return;
                            }
                            else
                                return;
                        }
                    }

                    // If on streak then we can move forward destroying enemy checkers
                    if (isStreak)
                    {
                        MoveDestroyingLowerLeft(checker);
                        MoveDestroyingLowerRight(checker);
                    }
                    else
                    {
                        MoveLowerLeft(checker);
                        MoveLowerRight(checker);
                        MoveDestroyingLowerLeft(checker);
                        MoveDestroyingLowerRight(checker);
                    }
                }
            }
        }
        #endregion
        // If selected checker is a king
        #region KingMovement
        else
        {
            AddKingPossibleMoves(checker);
        }
        #endregion
    }

    private static void MoveLowerRight(Checker checker)
    {
        // Move lower right
        if (checker.x < 7 &&
            PaintBoard.checkersMatrix[checker.y + 1, checker.x + 1] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y + 1 && x.x == checker.x + 1), null);
        }
    }

    private static void MoveLowerLeft(Checker checker)
    {
        // Move lower left
        if (checker.x > 0 &&
        PaintBoard.checkersMatrix[checker.y + 1, checker.x - 1] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y + 1 && x.x == checker.x - 1), null);
        }
    }

    private static void MoveUpperRight(Checker checker)
    {
        // Move upper right
        if (checker.x < 7 &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x + 1] == null)
        {
            var s = PaintBoard.squares.Find(x => x.y == checker.y - 1 && x.x == checker.x + 1);
            checker.possibleSquares.Add(s, null);
        }
    }

    private static void MoveUpperLeft(Checker checker)
    {
        // Move upper left
        if (checker.x > 0 &&
            PaintBoard.checkersMatrix[checker.y - 1, checker.x - 1] == null)
        {
            checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == checker.y - 1 && x.x == checker.x - 1), null);
        }
    }

    /// <summary>
    /// King moves at all diagonal directions without restrictions
    /// </summary>
    /// <param name="checker">Adds movement for this checker</param>
    /// <param name="isAddingPossibleCheckers">Needed to fill possibleCheckers list</param>
    private void AddKingPossibleMoves(Checker checker, bool isAddingPossibleCheckers = false)
    {
        if (!checker.isKing)
            return;

        CheckerColor color = checker.color;
        CheckerColor oppositeColor = (color == CheckerColor.White) ? CheckerColor.Black : CheckerColor.White;
        bool isDestroying = false;
        Checker checkerToDestroy = null;
        // Diagonal left upper move
        int i = checker.y - 1;
        int j = checker.x - 1;

        while (i >= 0 && j >= 0)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (PaintBoard.checkersMatrix[i, j] != null && isDestroying)
            {
                break;
            }
            // If current square is empty
            if (PaintBoard.checkersMatrix[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), checkerToDestroy);
                else
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), null);
            }
            // If we meet enemy checker then mark it and go further
            if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == oppositeColor)
            {
                if (i - 1 >= 0 && j - 1 >= 0 && PaintBoard.checkersMatrix[i - 1, j - 1] == null)
                {
                    isDestroying = true;
                    checkerToDestroy = PaintBoard.checkersMatrix[i, j];
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == color)
            {
                break;
            }

            i--;
            j--;
        }

        // Diagonal right upper move
        isDestroying = false;
        checkerToDestroy = null;
        i = checker.y - 1;
        j = checker.x + 1;
        while (i >= 0 && j <= 7)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (PaintBoard.checkersMatrix[i, j] != null && isDestroying)
            {
                break;
            }

            if (PaintBoard.checkersMatrix[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), checkerToDestroy);
                else
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), null);
            }
            // If we meet enemy checker then mark it and go further
            if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == oppositeColor)
            {
                if (i - 1 >= 0 && j + 1 <= 7 && PaintBoard.checkersMatrix[i - 1, j + 1] == null)
                {
                    isDestroying = true;
                    checkerToDestroy = PaintBoard.checkersMatrix[i, j];
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == color)
            {
                break;
            }

            i--;
            j++;
        }

        // Diagonal left lower move
        isDestroying = false;
        checkerToDestroy = null;
        i = checker.y + 1;
        j = checker.x - 1;
        while (i <= 7 && j >= 0)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (PaintBoard.checkersMatrix[i, j] != null && isDestroying)
            {
                break;
            }

            if (PaintBoard.checkersMatrix[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), checkerToDestroy);
                else
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), null);
            }
            // If we meet enemy checker then mark it and go further
            if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == oppositeColor)
            {
                if (i + 1 <= 7 && j - 1 >= 0 && PaintBoard.checkersMatrix[i + 1, j - 1] == null)
                {
                    isDestroying = true;
                    checkerToDestroy = PaintBoard.checkersMatrix[i, j];
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == color)
            {
                break;
            }

            i++;
            j--;
        }

        // Diagonal right lower move
        isDestroying = false;
        checkerToDestroy = null;
        i = checker.y + 1;
        j = checker.x + 1;
        while (i <= 7 && j <= 7)
        {
            // If we are already killing enemy and meet other checker on way then stop
            if (PaintBoard.checkersMatrix[i, j] != null && isDestroying)
            {
                break;
            }

            if (PaintBoard.checkersMatrix[i, j] == null)
            {
                if (isDestroying)
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), checkerToDestroy);
                else
                    checker.possibleSquares.Add(PaintBoard.squares.Find(x => x.y == i && x.x == j), null);
            }
            // If we meet enemy checker then mark it and go further
            if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == oppositeColor)
            {
                if (i + 1 <= 7 && j + 1 <= 7 && PaintBoard.checkersMatrix[i + 1, j + 1] == null)
                {
                    isDestroying = true;
                    checkerToDestroy = PaintBoard.checkersMatrix[i, j];
                }
                else
                    break;
            }
            // If we meet ally checker then stop
            else if (PaintBoard.checkersMatrix[i, j] != null && PaintBoard.checkersMatrix[i, j].color == color)
            {
                break;
            }

            i++;
            j++;
        }

        // Remain only destroying moves
        if (!isAddingPossibleCheckers)
        {
            if (possibleCheckers.Count > 0)
            {
                if (possibleCheckers.Contains(checker))
                {
                    ListDictionary temp = new ListDictionary();
                    foreach (DictionaryEntry de in checker.possibleSquares)
                    {
                        if (de.Value != null)
                        {
                            temp.Add(de.Key, de.Value);
                        }
                    }
                    checker.possibleSquares = temp;
                }
            }
        }
    }

    private void HighlightPossibleSquares()
    {
        if (selectedChecker != null)
        {
            foreach (DictionaryEntry square in selectedChecker.possibleSquares)
            {
                var sq = (Square)square.Key;
                var renderer = sq.gameObject.GetComponent<SpriteRenderer>();
                renderer.color = Color.yellow;
            }
        }
    }

    /// <summary>
    /// Highlights possible checkers to select. If any checker can beat enemy, than they are only option to select
    /// </summary>
    private void HightlightPossibleCheckers()
    {
        if (isWhiteTurn)
        {
            // If possibleCheckers has at least 1 element, than only use elements of this list
            if (possibleCheckers.Count > 0)
            {
                foreach (Checker ch in possibleCheckers)
                {
                    ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                foreach (Checker ch in PaintBoard.whiteCheckers)
                {
                    AddPossibleMoves(ch);
                    if (ch.possibleSquares.Count > 0)
                    {
                        ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
        }
        // If black turn
        else
        {
            // If possibleCheckers has at least 1 element, than only use elements of this list
            if (possibleCheckers.Count > 0)
            {
                foreach (Checker ch in possibleCheckers)
                {
                    ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                foreach (Checker ch in PaintBoard.blackCheckers)
                {
                    AddPossibleMoves(ch);
                    if (ch.possibleSquares.Count > 0)
                    {
                        ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
        }
    }

    private void ReturnSquaresColor(Checker checker)
    {
        if (checker == null) return;
        foreach (DictionaryEntry square in checker.possibleSquares)
        {
            var sq = (Square)square.Key;
            var renderer = sq.gameObject.GetComponent<SpriteRenderer>();
            renderer.color = Color.black;
        }
    }

    private void ReturnCheckersColor()
    {
        // Before ending turn return checkers to normal color
        if (isWhiteTurn)
        {
            foreach (Checker ch in PaintBoard.whiteCheckers)
            {
                ch.SquareUnderChecker.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            foreach (Checker ch in PaintBoard.blackCheckers)
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
    private void MoveChecker(Checker checker, Square square, Checker checkerToRemove = null)
    {
        // Move checker to selected square
        checker.transform.position = ExtensionMethods.GetVectorPosition(square.x, square.y);
        var zc = checker.transform.position;
        zc.z = -1f;
        checker.transform.position = zc;
        // If jump over the enemy checker then kill
        if (checkerToRemove != null)
        {
            DestroyChecker(checkerToRemove);
        }
        // Change status of squares
        PaintBoard.checkersMatrix[checker.y, checker.x] = null;
        PaintBoard.checkersMatrix[square.y, square.x] = checker;
        checker.x = square.x;
        checker.y = square.y;
        checker.SquareUnderChecker = square;
    }

    private void DestroyChecker(Checker checker)
    {
        if (checker != null)
        {
            PaintBoard.checkersMatrix[checker.y, checker.x] = null;
            if (checker.color == CheckerColor.White)
            {
                PaintBoard.whiteCheckers.Remove(checker);
            }
            else
            {
                PaintBoard.blackCheckers.Remove(checker);
            }
            Destroy(checker.gameObject);
        }
    }

    /// <summary>
    /// Iterates over all checkers of current turn to check if any of them can beat enemy.
    /// If there is, than add this checker to possibleCheckers list
    /// </summary>
    private void FindBeatingCheckers()
    {
        possibleCheckers.Clear();
        if (isWhiteTurn)
        {
            foreach (Checker ch in PaintBoard.whiteCheckers)
            {
                AddPossibleMoves(ch, true);
                foreach (DictionaryEntry sq in ch.possibleSquares)
                {
                    // If there at least 1 checker we can beat, then add it to possibleCheckers
                    if (sq.Value != null)
                    {
                        possibleCheckers.Add(ch);
                    }
                }
            }
        }
        else
        {
            foreach (Checker ch in PaintBoard.blackCheckers)
            {
                AddPossibleMoves(ch, true);
                foreach (DictionaryEntry sq in ch.possibleSquares)
                {
                    // If there at least 1 checker we can beat, then add it to possibleCheckers
                    if (sq.Value != null)
                    {
                        possibleCheckers.Add(ch);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called during a streak. You can end turn on any streak move
    /// </summary>
    private void EndMove()
    {
        EndTurnButton.SetActive(true);
        CheckVictory();
        selectedChecker = null;
        selectedSquare = null;
        FindBeatingCheckers();
        HightlightPossibleCheckers();
    }

    public void EndTurn()
    {
        EndTurnButton.SetActive(false);
        isStreak = false;
        CheckVictory();
        ReturnCheckersColor();
        selectedChecker = null;
        selectedSquare = null;
        isWhiteTurn = !isWhiteTurn;
        turnText.ChangeTurn();
        FindBeatingCheckers();
        HightlightPossibleCheckers();
    }

    private void CheckVictory()
    {
        if (PaintBoard.whiteCheckers.Count == 0)
        {
            gameOver.HandleOnGameOverEvent(CheckerColor.Black);
        }
        else if (PaintBoard.blackCheckers.Count == 0)
        {
            gameOver.HandleOnGameOverEvent(CheckerColor.White);
        }
    }
}
