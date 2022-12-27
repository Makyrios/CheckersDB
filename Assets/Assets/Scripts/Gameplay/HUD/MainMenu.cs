using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //private void Awake()
    //{
    //    if (DataBaseInitializer.singleton.userService.SelectAllUsers().Count == 0)
    //    {
    //        DataBaseInitializer.singleton.userService.WriteToDB();
    //        BaseGameAccount.IDSeed = DataBaseInitializer.singleton.userService.GetUserIDSeed();
    //        Game.gameIDSEED = DataBaseInitializer.singleton.userService.GetGameIDSeed();
    //    }
    //}

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

}
