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

    public bool isDebug;
    // Start is called before the first frame update
    void Start()
    {
        whitePlayerText.text = SelectPlayers.WhiteCheckersPlayer.Username;
        blackPlayerText.text = SelectPlayers.BlackCheckersPlayer.Username;

        WhiteWinButton.SetActive(false);
        BlackWinButton.SetActive(false);
        gameOver = FindObjectOfType<GameOver>();
        CreateButtons();
    }

    private void CreateButtons()
    {
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
