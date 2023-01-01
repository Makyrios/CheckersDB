using System;
using System.Linq;
using System.Text;

namespace GameClasses
{
    // Account gets additional points for win streak
    public class StreakGameAccount : BaseGameAccount
    {
        // Necessary count of won games. 1 = every first game, 2 = every second game etc.
        private const uint StreakCount = 2;
        // Bonus points for streak
        private const int BonusPoints = 50;

        public StreakGameAccount() { }
        public StreakGameAccount(string username) : base(username) { }
        public StreakGameAccount(int id, string username) : base(id, username) { }

        private bool CheckWinStreak(Game game)
        {
            int currentStreak = 0;
            bool isStreak = false;
            if (allGames.Count() >= StreakCount)
            {
                // Check if last N not training games were won
                for (int i = allGames.FindIndex(g => g == game); i >= 0; i--)
                {
                    var currentGame = allGames[i];
                    // If game is currentGame is already checked isStreak, then no need to check further
                    if (currentGame.IsStreak)
                    {
                        isStreak = true;
                        break;
                    }
                    // Check if current game is not training game
                    if (currentGame.GameType == GameType.TrainingGame)
                        continue;
                    // If current game is won for player than add streak
                    if (DataBaseInitializer.singleton.userService.GetPlayerByUsername(currentGame.Player1) == this)
                    {
                        currentStreak++;
                    }
                    // Check while current game is not lost or while current streak < StreakCount
                    if (DataBaseInitializer.singleton.userService.GetPlayerByUsername(currentGame.Player1) != this || currentStreak >= StreakCount)
                    {
                        if (currentStreak >= StreakCount)
                        {
                            isStreak = true;
                        }
                        break;
                    }
                }
            }
            return isStreak;
        }

        public override int CurrentRating
        {
            get
            {
                int rating = 1;
                foreach (var item in allGames)
                {
                    rating = ChangeRating(item, rating);
                    bool isStreak = CheckWinStreak(item);
                    if (isStreak)
                        rating += BonusPoints;

                    if (rating < 1)
                        rating = 1;
                }
                currentRating = rating;
                return rating;
            }
        }

        public override Game WinGame(GameType type, BaseGameAccount opponent, int rating = 0)
        {
            if (type != GameType.TrainingGame)
            {
                var game = base.WinGame(type, opponent, rating);
                bool isStreak = CheckWinStreak(game);
                if (isStreak)
                    game.IsStreak = true;
                return game;
            }
            return base.WinGame(type, opponent, rating);
        }

        public override string GetStats()
        {
            Console.WriteLine($"Statistics for {Username}");
            var report = new StringBuilder();

            int rating = 0;
            report.AppendLine($"{"GameID",-8}\t{"GameType",-17}\t{"Opponent",-11}\t{"Rating",-8}\t{"Result",-8}");
            foreach (var item in allGames)
            {
                rating = ChangeRating(item, rating);
                string opponent = (DataBaseInitializer.singleton.userService.GetPlayerByUsername(item.Player1) == this) ? item.Player2 : item.Player1;
                string getResult = (DataBaseInitializer.singleton.userService.GetPlayerByUsername(item.Player1) == this) ? "Won" : "Lose";
                string bonusPointLine = (item.IsStreak && DataBaseInitializer.singleton.userService.GetPlayerByUsername(item.Player1) == this) ? "+" + BonusPoints.ToString() : "";
                report.AppendLine($"{item.ID,-8}\t{item.GameType,-17}\t{opponent,-11}\t{item.Rating + bonusPointLine,-8}\t{getResult,-8}");
            }
            return report.ToString();
        }
    }
}
