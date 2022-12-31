using GameClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI usernameColumn;
    public TextMeshProUGUI scoreColumn;
    private List<BaseGameAccount> sortedUsers;

    private void Start()
    {
        sortedUsers = DataBaseInitializer.singleton.userService.SelectAllUsers().ToList<BaseGameAccount>();
        sortedUsers.Sort((a, b) => b.CurrentRating.CompareTo(a.CurrentRating));
        ShowUsernames();
        ShowScores();
    }

    private void ShowUsernames()
    {
        StringBuilder usernames = new StringBuilder();

        foreach (var user in sortedUsers)
        {
            usernames.AppendLine(user.Username);
        }
        usernameColumn.text = usernames.ToString();
    }

    private void ShowScores()
    {
        StringBuilder usernames = new StringBuilder();
        foreach (var user in sortedUsers)
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
