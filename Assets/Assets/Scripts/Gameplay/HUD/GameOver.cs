using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static CheckerColor winner;
    public void HandleOnGameOverEvent(CheckerColor color)
    {
        if (color == CheckerColor.White)
        {
            SelectPlayers.WhiteCheckersPlayer.WinGame(SelectPlayers.CurrentGameType, SelectPlayers.BlackCheckersPlayer, SelectPlayers.CurrentRating);
        }
        else
        {
            SelectPlayers.BlackCheckersPlayer.WinGame(SelectPlayers.CurrentGameType, SelectPlayers.WhiteCheckersPlayer, SelectPlayers.CurrentRating);
        }
        winner = color;
        SceneManager.LoadScene("GameOverScene");
    }

    public void HandleOnReturnButtonClickEvent()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
