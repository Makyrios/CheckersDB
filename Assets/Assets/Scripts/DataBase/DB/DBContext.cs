using GameClasses;
using System.Collections.Generic;

public class DBContext
{
    public int GameIDSeed { get; set; } = 1234567;
    public int UserIDSeed { get; set; } = 12345;

    public List<BaseGameAccount> Users { get; set; }
    public List<Game> GamesHistory { get; set; }

    public DBContext()
    {
        Users = new List<BaseGameAccount>();
        GamesHistory = new List<Game>();
    }
}
