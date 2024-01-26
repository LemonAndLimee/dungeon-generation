using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;

    protected override void RunProceduralGeneration()
    {
        CreateCorridors();
    }

    private void CreateCorridors()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        var currentPosition = startPosition;
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.SimpleRandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            floorPositions.UnionWith(corridor);
        }

        tilemapVisualiser.PaintFloorTiles(floorPositions);
    }
}
