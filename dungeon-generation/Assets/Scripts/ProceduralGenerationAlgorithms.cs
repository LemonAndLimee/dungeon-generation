using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static List<Vector2Int> SimpleRandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
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
