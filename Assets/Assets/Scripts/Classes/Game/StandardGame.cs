using System;

namespace GameClasses
{
    // Standart game on certain rating
    class StandardGame : Game
    {
        public StandardGame() { }

        public StandardGame(int rating, BaseGameAccount p1, BaseGameAccount p2) : base(p1, p2)
        {
            if (rating < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be positive value");
            }

            this.rating = rating;
            gameType = GameType.StandartGame;
        }

    }
}
