using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClasses
{
    public enum GameAccount
    {
        Standart,
        Bonus,
        Streak
    }

    public class BaseGameAccount
    {
        public static int IDSeed;
        protected GameAccount type;
        public int id;
        public string Username;
        protected int currentRating;
        protected int gamesCount;

        [JsonIgnore]
        public int ID { get { return id; } }

        public List<Game> allGames;

        private GameFactory gf = new GameFactory();

        public virtual int CurrentRating
        {
            get
            {
                int rating = 1;
                foreach (var item in allGames)
                {
                    rating = ChangeRating(item, rating);
                    if (rating < 1)
                        rating = 1;
                }

                currentRating = rating;
                return rating;
            }
        }
        public int GamesCount
        {
            get
            {
                int count = 0;
                foreach (var item in allGames)
                {
                    count++;
                }

                gamesCount = count;
                return count;
            }
        }

        public BaseGameAccount() { }

        public BaseGameAccount(string userName)
        {
            allGames = new List<Game>();
            type = GameAccount.Standart;
            Username = userName;
            id = IDSeed++;
        }

        public BaseGameAccount(int id, string username)
        {
            allGames = new List<Game>();
            type = GameAccount.Standart;
            Username = username;
            this.id = id;
        }


        protected int ChangeRating(Game game, int rating)
        {
            if (DataBaseInitializer.singleton.userService.GetPlayerByUsername(game.Player1) == this)
            {
                rating += game.Rating;
            }
            else
            {
                rating -= game.Rating;
            }
            return rating;
        }

        public virtual Game WinGame(GameType type, BaseGameAccount opponent, int rating = 0)
        {
            if (opponent == this)
            {
                throw new ArgumentException("You cannot play with yourself");
            }
            Game game = gf.CreateGame(type, this, opponent, rating);
            DataBaseInitializer.singleton.userService.CreateGame(game);
            allGames.Add(game);
            opponent.allGames.Add(game);
            return game;
        }

        public virtual string GetStats()
        {
            Console.WriteLine($"Statistics for {Username}");
            var report = new StringBuilder();

            int rating = 0;
            report.AppendLine($"{"GameID",10}{"GameType",20}{"Opponent",20}{"Rating",15}{"Result",10}");
            foreach (var item in allGames)
            {
                rating = ChangeRating(item, rating);
                var opponent = (DataBaseInitializer.singleton.userService.GetPlayerByUsername(item.Player1) == this) ? item.Player2 : item.Player1;
                string getResult = DataBaseInitializer.singleton.userService.GetPlayerByUsername(item.Player1) == this ? "Won" : "Lose";
                report.AppendLine($"{item.ID,10}{item.GameType,20}{opponent,20}{item.Rating,15}{getResult,10}");
            }
            return report.ToString();
        }
    }


}
