using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static List<Vector2Int> cardinalDirectionsListWithDiagonals = new List<Vector2Int>
    {
        Vector2Int.up,
        new Vector2Int(1, 1),
        Vector2Int.right,
        new Vector2Int(1, -1),
        Vector2Int.down,
        new Vector2Int(-1, -1),
        Vector2Int.left,
        new Vector2Int(-1, 1)
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }

    public static Vector2Int Rotate(Vector2Int vector, int degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        Vector2 rotated = new Vector2(
            (cos * vector.x) - (sin * vector.y),
            (sin * vector.x) + (cos * vector.y) );
        return Vector2Int.RoundToInt(rotated);
    }

    public static int GetAngleBetweenVectors(Vector2Int vector1, Vector2Int vector2)
    {
        int angle = Mathf.RoundToInt(Vector2.SignedAngle(vector1, vector2));
        if (angle < 0)
        {
            angle = 360 + angle;
        }

        return angle;
    }
}
