using GameClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DB
{
    public class UserService : IUserService
    {
        private static string UserPath = "Users.txt";
        private static string GamesPath = "Games.txt";
        private GameFactory gf = new GameFactory();
        public DBContext DBContext_ { get; private set; }
        public UserService(DBContext dBContext)
        {
            DBContext_ = dBContext;
        }
        public void CreateUser(BaseGameAccount user)
        {
            DBContext_.Users.Add(user);
        }

        public void CreateGame(Game game)
        {
            DBContext_.GamesHistory.Add(game);
        }

        public List<BaseGameAccount> SelectAllUsers()
        {
            return DBContext_.Users;
        }

        public List<Game> SelectAllGames()
        {
            return DBContext_.GamesHistory;
        }

        public List<BaseGameAccount> SelectBonusAccounts()
        {

            return DBContext_.Users.Where(x => x is BonusGameAccount).ToList();
        }

        public List<BaseGameAccount> SelectStreakAccounts()
        {

            return DBContext_.Users.Where(x => x is StreakGameAccount).ToList();
        }

        public BaseGameAccount GetPlayerByUsername(string username)
        {
            return DBContext_.Users.Find(x => x.Username == username);
        }

        public BaseGameAccount GetPlayerByID(int id)
        {
            return DBContext_.Users.Find(x => x.ID == id);
        }

        public List<int> GetIds()
        {
            return DBContext_.Users.Select(x => x.ID).ToList();
        }

        public int GetGameIDSeed()
        {
            return DBContext_.GameIDSeed;
        }

        public int GetUserIDSeed()
        {
            return DBContext_.UserIDSeed;
        }

        public void WriteToDB()
        {
            // Read players from text
            try
            {
                using (StreamReader readtext = File.OpenText(UserPath))
                {
                    UnityEngine.Debug.Log("Users file opened");
                    BaseGameAccount newPlayer;
                    string s = "";
                    while ((s = readtext.ReadLine()) != null)
                    {
                        string type = s;
                        s = readtext.ReadLine();
                        int id = Convert.ToInt32(s);
                        s = readtext.ReadLine();
                        string username = s;

                        switch (type)
                        {
                            case "BaseGameAccount":
                                newPlayer = new BaseGameAccount(id, username);
                                break;
                            case "StreakGameAccount":
                                newPlayer = new StreakGameAccount(id, username);
                                break;
                            case "BonusGameAccount":
                                newPlayer = new BonusGameAccount(id, username);
                                break;
                            default:
                                newPlayer = null;
                                break;
                        }

                        DBContext_.Users.Add(newPlayer);
                        // Seed equals last player's id
                        DBContext_.UserIDSeed = ++id;
                    }
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Couldn't open users file " + e);
            }

            // Read games from text
            try
            {
                using (StreamReader readtext = File.OpenText(GamesPath))
                {
                    UnityEngine.Debug.Log("Games file opened");
                    string s = "";
                    while ((s = readtext.ReadLine()) != null)
                    {
                        string type = s;
                        s = readtext.ReadLine();
                        int id = Convert.ToInt32(s);
                        s = readtext.ReadLine();
                        int player1 = Convert.ToInt32(s);
                        s = readtext.ReadLine();
                        int player2 = Convert.ToInt32(s);
                        s = readtext.ReadLine();
                        int rating = Convert.ToInt32(s);
                        s = readtext.ReadLine();
                        bool isStreak = Convert.ToBoolean(s);

                        GameType gameType;
                        switch (type)
                        {
                            case "StandartGame":
                                gameType = GameType.StandartGame;
                                break;
                            case "TrainingGame":
                                gameType = GameType.TrainingGame;
                                break;
                            case "AllInRatingGame":
                                gameType = GameType.AllInRatingGame;
                                break;
                            default:
                                gameType = GameType.StandartGame;
                                break;
                        }
                        Game addGame = gf.CreateGame(id, gameType, DBContext_.Users.Find(p1 => p1.ID == player1), DBContext_.Users.Find(p2 => p2.ID == player2), rating);
                        DBContext_.GamesHistory.Add(addGame);
                        // Seed equals last player's id
                        DBContext_.GameIDSeed = ++id;
                    }
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Couldn't open games file " + e);
            }

            // Update allGames list to all players
            foreach (var player in DBContext_.Users)
            {
                player.allGames = GetAllGamesList(player.ID);
            }

        }

        private List<Game> GetAllGamesList(int id)
        {
            List<Game> list = new List<Game>();
            foreach (var game in DBContext_.GamesHistory)
            {
                if (game.Player1.ID == id || game.Player2.ID == id)
                {
                    list.Add(game);
                }
            }
            return list;
        }

        public void WriteToFile()
        {
            using (StreamWriter writetext = new StreamWriter(UserPath, false))
            {
                foreach (BaseGameAccount account in DataBaseInitializer.singleton.userService.SelectAllUsers())
                {
                    string type = account.GetType().Name;
                    int id = account.ID;
                    string username = account.Username;
                    int rating = account.CurrentRating;
                    int games = account.GamesCount;

                    writetext.WriteLine(type);
                    writetext.WriteLine(id);
                    writetext.WriteLine(username);
                }
            }

            using (StreamWriter writetext = new StreamWriter(GamesPath))
            {
                foreach (Game game in DataBaseInitializer.singleton.userService.SelectAllGames())
                {
                    GameType type = game.GameType;
                    int id = game.ID;
                    int player1 = game.Player1.ID;
                    int player2 = game.Player2.ID;
                    int rating = game.Rating;
                    bool isStreak = game.IsStreak;

                    writetext.WriteLine(type);
                    writetext.WriteLine(id);
                    writetext.WriteLine(player1);
                    writetext.WriteLine(player2);
                    writetext.WriteLine(rating);
                    writetext.WriteLine(isStreak);
                }
            }
        }

    }
}


