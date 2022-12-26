using UnityEngine;

public class Square : MonoBehaviour
{
    public int x;
    public int y;

    public Square(Square s)
    {
        this.x = s.x;
        this.y = s.y;
    }
}
