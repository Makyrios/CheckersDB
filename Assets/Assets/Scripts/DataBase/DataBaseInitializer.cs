using DB;
using GameClasses;
using System.Linq;
using UnityEngine;

public class DataBaseInitializer : MonoBehaviour
{
    public static DataBaseInitializer singleton { get; private set; }
    private DBContext dbcontext;

    public UserService userService { get; private set; }
    public BaseGameAccount CurrentPlayer { get; set; }


    private void Awake()
    {
        if (!singleton)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    DataBaseInitializer()
    {
        dbcontext = new DBContext();
        userService = new UserService(dbcontext);
    }

    private void OnApplicationQuit()
    {
        if (userService.DBContext_.Users.Any())
        {
            userService.DBContext_.UserIDSeed = userService.DBContext_.Users.Last().ID + 1;
        }
        if (userService.DBContext_.GamesHistory.Any())
        {
            userService.DBContext_.GameIDSeed = userService.DBContext_.GamesHistory.Last().ID + 1;
        }
        userService.WriteToFile();
    }

}
