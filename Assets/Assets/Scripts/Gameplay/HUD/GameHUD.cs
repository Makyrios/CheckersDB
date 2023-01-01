using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField]
    GameObject WhiteWinButton;
    [SerializeField]
    GameObject BlackWinButton;

    [SerializeField]
    TextMeshProUGUI whitePlayerText;
    [SerializeField]
    TextMeshProUGUI blackPlayerText;

    GameOver gameOver;
    private bool isDebug;



    // Start is called before the first frame update
    void Start()
    {
        isDebug = true;
        whitePlayerText.text = SelectPlayers.WhiteCheckersPlayer.Username;
        blackPlayerText.text = SelectPlayers.BlackCheckersPlayer.Username;

        gameOver = FindObjectOfType<GameOver>();
        CreateDebugButtons();
    }

    private void CreateDebugButtons()
    {
        WhiteWinButton.SetActive(false);
        BlackWinButton.SetActive(false);
        if (isDebug)
        {
            WhiteWinButton.SetActive(true);
            BlackWinButton.SetActive(true);
        }
    }

    public void HandleOnWhiteWinButtonEvent()
    {
        gameOver.HandleOnGameOverEvent(CheckerColor.White);
    }

    public void HandleOnBlackWinButtonEvent()
    {
        gameOver.HandleOnGameOverEvent(CheckerColor.Black);
    }
}
