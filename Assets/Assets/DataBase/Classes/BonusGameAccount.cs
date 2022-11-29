using GameClasses;
using System.Collections.Generic;

// For won games get 2x points, for lost games lose 0.5x points
public class BonusGameAccount : BaseGameAccount
{
    public BonusGameAccount(string username) : base(username) { }
    public BonusGameAccount(int id, string username) : base(id, username) { }


    public override int CurrentRating
    {
        get
        {
            int rating = 1;
            foreach (var item in allGames)
            {
                int changeRating = ChangeRating(item, rating);
                if (changeRating >= rating)
                {
                    rating += item.Rating * 2;
                }
                else
                {
                    rating -= item.Rating / 2;
                }
                if (rating < 1)
                    rating = 1;
            }


            return rating;
        }
    }
}
