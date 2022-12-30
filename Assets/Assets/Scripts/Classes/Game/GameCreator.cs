using GameClasses;

public abstract class GameCreator
{
    public abstract Game FactoryMethod(BaseGameAccount p1, BaseGameAccount p2, int rating = 0);
}

public class StandardGameCreator : GameCreator
{
    public override Game FactoryMethod(BaseGameAccount p1, BaseGameAccount p2, int rating = 0)
    {
        return new StandardGame(rating, p1, p2);
    }
}

public class TrainingGameCreator : GameCreator
{
    public override Game FactoryMethod(BaseGameAccount p1, BaseGameAccount p2, int rating = 0)
    {
        return new TrainingGame(p1, p2);
    }
}

public class AllInRatingGameCreator : GameCreator
{
    public override Game FactoryMethod(BaseGameAccount p1, BaseGameAccount p2, int rating = 0)
    {
        return new AllInRatingGame(p1, p2);
    }
}
