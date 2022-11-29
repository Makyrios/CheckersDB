using GameClasses;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private void Awake()
    {
        if (DataBaseInitializer.singleton.userService.SelectAllUsers().Count == 0)
        {
            DataBaseInitializer.singleton.userService.WriteToDB();
            BaseGameAccount.IDSeed = DataBaseInitializer.singleton.userService.GetUserIDSeed();
            Game.gameIDSEED = DataBaseInitializer.singleton.userService.GetGameIDSeed();
        }
    }
    
    public void HandleOnPlayButtonEvent()
    {
        SceneManager.LoadScene("SelectPlayersScene");
    }

    public void HandleOnAddUserButtonEvent()
    {
        SceneManager.LoadScene("AddPlayerScene");
    }

    public void HandleOnPlayerStatsButtonEvent()
    {
        SceneManager.LoadScene("PlayerStatsScene");
    }

    public void HandleQuitButtonOnEvent()
    {
        Application.Quit();
    }


    //private void OnApplicationQuit()
    //{
    //    using (StreamWriter writetext = File.AppendText(DBContext.UserPath))
    //    {
    //        foreach (BaseGameAccount account in DataBaseInitializer.userService.SelectAll())
    //        {
    //            string type = account.GetType().Name;
    //            int id = account.ID;
    //            string username = account.UserName;
    //            int rating = account.CurrentRating;
    //            int games = account.GamesCount;
    //            List<Game> gamesHistory = account.allGames;

    //            writetext.WriteLine(type);
    //            writetext.WriteLine(id);
    //            writetext.WriteLine(username);
    //            writetext.WriteLine(rating);
    //            writetext.WriteLine(games);
    //            writetext.WriteLine(gamesHistory);
    //        }
    //    }
    //}

}
