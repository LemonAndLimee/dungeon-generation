using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualiser tilemapVisualiser)
    {
        var wallPositions = FindWalls(floorPositions, Direction2D.cardinalDirectionsListWithDiagonals);
        tilemapVisualiser.PaintWallTiles(wallPositions);
    }

    public static HashSet<Vector2Int> FindWalls(HashSet<Vector2Int> floorPositions, List<Vector2Int> directions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directions)
            {
                var adjacentPosition = position + direction;
                if (!floorPositions.Contains(adjacentPosition))
                {
                    wallPositions.Add(adjacentPosition);
                }
            }
        }
        return wallPositions;
    }
}
