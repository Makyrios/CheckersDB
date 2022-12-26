using GameClasses;
using System.Collections.Generic;

namespace DB
{
    public interface IUserService
    {
        public void CreateUser(BaseGameAccount user);
        public void CreateGame(Game game);
        public List<BaseGameAccount> SelectAllUsers();
        public List<Game> SelectAllGames();
        public List<int> GetIds();
        public void WriteToDB();
        public void WriteToFile();
    }
}
