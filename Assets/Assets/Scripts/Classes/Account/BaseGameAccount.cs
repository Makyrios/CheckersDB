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
        public static int IDSeed = DataBaseInitializer.singleton.userService.GetUserIDSeed();
        public int id;
        public string Username;
        protected int currentRating;
        protected int gamesCount;

        [JsonIgnore]
        public int ID { get { return id; } }

        public List<Game> allGames;

        private GameCreator gc;

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
            Username = userName;
            id = IDSeed++;
        }

        public BaseGameAccount(int id, string username)
        {
            allGames = new List<Game>();
            Username = username;
            this.id = id;
        }

        private GameCreator GetFactoryCreator(GameType type)
        {
            if (type == GameType.StandartGame)
            {
                return new StandardGameCreator();
            }
            else if (type == GameType.TrainingGame)
            {
                return new TrainingGameCreator();
            }
            else
            {
                return new AllInRatingGameCreator();
            }
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
            gc = GetFactoryCreator(type);
            Game game = gc.FactoryMethod(this, opponent, rating);
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
