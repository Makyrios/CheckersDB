using UnityEngine;

namespace GameClasses
{
    // Game on the lowest rating of two players
    class AllInRatingGame : Game
    {
        public AllInRatingGame() { }

        public AllInRatingGame(BaseGameAccount p1, BaseGameAccount p2) : base(p1, p2)
        {
            rating = Mathf.Min(p1.CurrentRating, p2.CurrentRating);
            gameType = GameType.AllInRatingGame;
        }
    }
}
