using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI usernameColumn;
    public TextMeshProUGUI scoreColumn;

    private void Start()
    {
        ShowUsernames();
        ShowScores();
    }

    private void ShowUsernames()
    {
        StringBuilder usernames = new StringBuilder();
        foreach (var user in DataBaseInitializer.singleton.userService.SelectAllUsers())
        {
            usernames.AppendLine(user.Username);
        }
        usernameColumn.text = usernames.ToString();
    }

    private void ShowScores()
    {
        StringBuilder usernames = new StringBuilder();
        foreach (var user in DataBaseInitializer.singleton.userService.SelectAllUsers())
        {
            usernames.AppendLine(user.CurrentRating.ToString());
        }
        scoreColumn.text = usernames.ToString();
    }

    public void HandleOnReturnButtonEvent()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
