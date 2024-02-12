using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CorridorGenerationAlgorithms;

public class CorridorDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;

    protected override void RunProceduralGeneration()
    {
        CorridorProperties corridorProperties = new CorridorProperties(startPosition, corridorLength, corridorCount);
        HashSet<Vector2Int> floorPositions = CreateCorridors(corridorProperties, new HashSet<Vector2Int>());
        tilemapVisualiser.PaintFloorTiles(floorPositions);
    }
}
