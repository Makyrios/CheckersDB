using System;

namespace GameClasses
{
    public enum GameType
    {
        StandartGame,
        TrainingGame,
        AllInRatingGame
    }

    public abstract class Game
    {
        // Fields
        public static int gameIDSEED;
        protected int id;
        protected int rating;
        protected BaseGameAccount player1;
        protected BaseGameAccount player2;
        protected GameType gameType;
        protected bool isStreak = false;

        // Properties
        public GameType GameType { get { return gameType; } }
        public int ID { get { return id; } }
        public int Rating { get { return rating; } }
        public BaseGameAccount Player1 { get { return player1; } }
        public BaseGameAccount Player2 { get { return player2; } }
        
        // bool field for StreakGameAccount to determine if game is in streak
        public bool IsStreak { get; set; }

        // Constructor
        public Game(BaseGameAccount p1, BaseGameAccount p2)
        {
            player1 = p1;
            player2 = p2;
            id = gameIDSEED++;
        }

        public Game(int id, BaseGameAccount p1, BaseGameAccount p2)
        {
            player1 = p1;
            player2 = p2;
            this.id = id;
        }
    }

    // Standart game on certain rating
    class StandardGame : Game
    {
        public StandardGame(int rating, BaseGameAccount p1, BaseGameAccount p2) : base(p1, p2)
        {
            if (rating < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be positive value");
            }

            this.rating = rating;
            gameType = GameType.StandartGame;
        }

        public StandardGame(int id, int rating, BaseGameAccount p1, BaseGameAccount p2) : base(id, p1, p2)
        {
            if (rating < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be positive value");
            }

            this.rating = rating;
            gameType = GameType.StandartGame;
        }
    }

    // Training game with no rating
    class TrainingGame : Game
    {
        public TrainingGame(BaseGameAccount p1, BaseGameAccount p2) : base(p1, p2)
        {
            rating = 0;
            gameType = GameType.TrainingGame;
        }

        public TrainingGame(int id, BaseGameAccount p1, BaseGameAccount p2) : base(id, p1, p2)
        {
            rating = 0;
            gameType = GameType.TrainingGame;
        }
    }

    // Game on the lowest rating of two players
    class AllInRatingGame : Game
    {
        public AllInRatingGame(BaseGameAccount p1, BaseGameAccount p2) : base(p1, p2)
        {
            rating = Math.Min(p1.CurrentRating, p2.CurrentRating);
            gameType = GameType.AllInRatingGame;
        }

        public AllInRatingGame(int id, BaseGameAccount p1, BaseGameAccount p2) : base(id, p1, p2)
        {
            rating = Math.Min(p1.CurrentRating, p2.CurrentRating);
            gameType = GameType.AllInRatingGame;
        }
    }

    // Factory class
    public class GameFactory
    {
        public Game CreateGame(int id, GameType type, BaseGameAccount p1, BaseGameAccount p2, int raiting = 0)
        {
            switch (type)
            {
                case GameType.StandartGame:
                    return new StandardGame(id, raiting, p1, p2);

                case GameType.TrainingGame:
                    return new TrainingGame(id, p1, p2);

                case GameType.AllInRatingGame:
                    return new AllInRatingGame(id, p1, p2);

                default:
                    return new TrainingGame(id, p1, p2);
            }
        }

        public Game CreateGame(GameType type, BaseGameAccount p1, BaseGameAccount p2, int raiting = 0)
        {
            switch (type)
            {
                case GameType.StandartGame:
                    return new StandardGame(raiting, p1, p2);

                case GameType.TrainingGame:
                    return new TrainingGame(p1, p2);

                case GameType.AllInRatingGame:
                    return new AllInRatingGame(p1, p2);

                default:
                    return new TrainingGame(p1, p2);
            }
        }
    }
}
