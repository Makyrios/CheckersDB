using DB;
using UnityEngine;

public class DataBaseInitializer : MonoBehaviour
{
    public static DataBaseInitializer singleton { get; private set; }
    private DBContext dbcontext;
    public UserService userService { get; private set; }


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
        userService.WriteToFile();
    }

}
