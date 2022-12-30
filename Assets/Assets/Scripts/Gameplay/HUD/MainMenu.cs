using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void HandleOnPlayButtonEvent()
    {
        SceneManager.LoadScene("SelectPlayersScene");
    }

    public void HandleOnAddUserButtonEvent()
    {
        SceneManager.LoadScene("AddPlayerScene");
    }

    public void HandleOnLeaderboardButtonEvent()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void HandleOnPlayerStatsButtonEvent()
    {
        SceneManager.LoadScene("PlayerStatsScene");
    }

    public void HandleQuitButtonOnEvent()
    {
        Application.Quit();
    }
}
