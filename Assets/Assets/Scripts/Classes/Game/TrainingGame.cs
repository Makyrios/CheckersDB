namespace GameClasses
{
    // Training game with no rating
    class TrainingGame : Game
    {
        public TrainingGame() { }

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
}
