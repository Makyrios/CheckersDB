using Newtonsoft.Json;

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
        public static int gameIDSEED = DataBaseInitializer.singleton.userService.GetGameIDSeed();
        protected int id;
        protected int rating;
        protected string player1;
        protected string player2;
        protected GameType gameType;
        protected bool isStreak = false;

        // Properties
        [JsonIgnore]
        public GameType GameType { get { return gameType; } }
        public int ID { get { return id; } set { if (id == 0) id = value; } }
        public int Rating { get { return rating; } set { if (rating == 0) rating = value; } }
        public string Player1 { get { return player1; } set { if (player1 == null) player1 = value; } }
        public string Player2 { get { return player2; } set { if (player2 == null) player2 = value; } }

        // bool field for StreakGameAccount to determine if game is in streak
        public bool IsStreak { get; set; }

        // Constructor
        public Game() { }

        public Game(BaseGameAccount p1, BaseGameAccount p2)
        {
            player1 = p1.Username;
            player2 = p2.Username;
            id = gameIDSEED++;
        }

    }

}
