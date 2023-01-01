using GameClasses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DB
{
    public class UserService : IUserService
    {
        private static string DataPath = "Data.json";
        private GameCreator gc;

        public DBContext DBContext_;
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

        public void ReadToDB()
        {
            if (!File.Exists(DataPath))
            {
                return;
            }

            using (StreamReader readtext = File.OpenText(DataPath))
            {
                string serializedObject = null;
                if (readtext.Peek() != -1)
                {
                    serializedObject = readtext.ReadToEnd();
                }
                else
                {
                    return;
                }
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                DBContext_ = JsonConvert.DeserializeObject<DBContext>(serializedObject, settings);
            }
            BaseGameAccount.IDSeed = DataBaseInitializer.singleton.userService.GetUserIDSeed();
            Game.gameIDSEED = DataBaseInitializer.singleton.userService.GetGameIDSeed();
        }


        public void WriteToFile()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
            var serializedObject = JsonConvert.SerializeObject(DBContext_, settings);
            using (StreamWriter write = new StreamWriter(DataPath, false))
            {
                write.WriteLine(serializedObject);
            }

        }

    }
}


