using DB;
using GameClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;

public class DBContext
{
    public int GameIDSeed { get; set; } = 123456789;
    public int UserIDSeed { get; set; } = 12345;

    public List<BaseGameAccount> Users { get; set; }
    public List<Game> GamesHistory { get; set; }

    public DBContext()
    {
        Users = new List<BaseGameAccount>();
        GamesHistory = new List<Game>();

    }
}
