using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckerColor
{
    White,
    Black
}

public static class ExtensionMethods
{
    public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static Vector2 GetXYPosition(Vector2 pos)
    {
        Vector2 result = new Vector2(
            Map(pos.x, PaintBoard.StartPosition.x, PaintBoard.EndPosition.x, 0, 7),
            Map(pos.y, PaintBoard.StartPosition.y, PaintBoard.EndPosition.y, 0, 7)
            );
        return result;
    }

    public static Vector2 GetVectorPosition(Vector2 pos)
    {
        Vector2 result = new Vector2(
            Map(pos.x, 0, 7, PaintBoard.StartPosition.x, PaintBoard.EndPosition.x),
            Map(pos.y, 0, 7, PaintBoard.StartPosition.y, PaintBoard.EndPosition.y)
            );
        return result;
    }

    public static Vector2 GetVectorPosition(int x, int y)
    {
        Vector2 result = new Vector2(
            Map(x, 0, 7, PaintBoard.spawnPositionStart.x, PaintBoard.spawnPositionEnd.x),
            Map(y, 0, 7, PaintBoard.spawnPositionStart.y, PaintBoard.spawnPositionEnd.y)
            );
        return result;
    }
}
