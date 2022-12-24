using UnityEngine;

public class PaintBoard : MonoBehaviour
{
    #region Prefabs

    [SerializeField]
    GameObject prefabWhiteSquare;
    [SerializeField]
    GameObject prefabBlackSquare;

    [SerializeField]
    GameObject prefabWhiteChecker;
    [SerializeField]
    GameObject prefabBlackChecker;

    #endregion

    #region StaticVariables
    public static Board currentBoard;
    #endregion

    #region SpawnVariables

    GameObject gameBoard;

    public static Vector2 StartPosition = new Vector2(-4.0f, 4.0f);
    public static Vector2 EndPosition = new Vector2(4.0f, -4.0f);

    public static Vector2 spawnPositionStart;
    public static Vector2 spawnPositionEnd;

    public static float squareHalfScale;

    #endregion



    // Start is called before the first frame update
    void Start()
    {
        currentBoard = new Board();

        gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        squareHalfScale = prefabBlackSquare.transform.localScale.x / 2;
        spawnPositionStart = new Vector2(StartPosition.x + squareHalfScale, StartPosition.y - squareHalfScale);
        spawnPositionEnd = new Vector2(EndPosition.x - squareHalfScale, EndPosition.y + squareHalfScale);

        GenerateBoard();
    }


    private void GenerateBoard()
    {
        bool isWhiteSquare = true;

        int x = 0, y = 0;
        for (y = 0; y < 8; y++)
        {
            for (x = 0; x < 8; x++)
            {
                Vector2 spawnPosition = new Vector2(
            ExtensionMethods.Map(x, 0, 7, spawnPositionStart.x, spawnPositionEnd.x),
            ExtensionMethods.Map(y, 0, 7, spawnPositionStart.y, spawnPositionEnd.y)
            );
                // Choose which square to spawn
                GameObject currentSquare;
                if (isWhiteSquare)
                    currentSquare = Instantiate(prefabWhiteSquare, spawnPosition, Quaternion.identity);
                else
                    currentSquare = Instantiate(prefabBlackSquare, spawnPosition, Quaternion.identity);
                currentSquare.transform.SetParent(gameBoard.transform, true);
                currentSquare.GetComponent<Square>().x = x;
                currentSquare.GetComponent<Square>().y = y;
                currentBoard.squares.Add(currentSquare.GetComponent<Square>());

                // Spawn checkers
                if (!isWhiteSquare)
                {
                    if (y <= 2)
                    {
                        var currentChecker = SpawnChecker(prefabBlackChecker, spawnPosition);
                        currentBoard.checkers[y, x] = currentChecker.GetComponent<Checker>();
                        currentChecker.GetComponent<Checker>().x = x;
                        currentChecker.GetComponent<Checker>().y = y;
                        currentChecker.GetComponent<Checker>().color = CheckerColor.Black;
                        currentChecker.GetComponent<Checker>().SquareUnderChecker = currentSquare.GetComponent<Square>();

                        currentBoard.blackCheckers.Add(currentChecker.GetComponent<Checker>());
                    }
                    else if (y >= 5)
                    {
                        var currentChecker = SpawnChecker(prefabWhiteChecker, spawnPosition);
                        currentBoard.checkers[y, x] = currentChecker.GetComponent<Checker>();
                        currentChecker.GetComponent<Checker>().x = x;
                        currentChecker.GetComponent<Checker>().y = y;
                        currentChecker.GetComponent<Checker>().color = CheckerColor.White;

                        currentChecker.GetComponent<Checker>().SquareUnderChecker = currentSquare.GetComponent<Square>();
                        currentBoard.whiteCheckers.Add(currentChecker.GetComponent<Checker>());
                    }
                }

                isWhiteSquare = !isWhiteSquare;
            }
            isWhiteSquare = y % 2 == 0 ? false : true;
        }
    }

    private GameObject SpawnChecker(GameObject obj, Vector2 pos)
    {
        var checker = Instantiate(obj, pos, Quaternion.identity);
        // Change z axis of checker to always hit its collider first
        var z = checker.transform.position;
        z.z += -1f;
        checker.transform.position = z;
        checker.transform.SetParent(gameBoard.transform, true);
        return checker;
    }


}
