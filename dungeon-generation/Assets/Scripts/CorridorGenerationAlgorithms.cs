using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CorridorGenerationAlgorithms
{
    public static List<Vector2Int> SimpleRandomWalkCorridor(CorridorProperties corridorProperties, HashSet<Vector2Int> positionsToAvoid)
    {
        HashSet<Vector2Int> untriedDirections = new HashSet<Vector2Int>(Direction2D.cardinalDirectionsList);

        while (untriedDirections.Count > 0)
        {
            List<Vector2Int> corridor = new List<Vector2Int>();
            var direction = untriedDirections.ElementAt(Random.Range(0, untriedDirections.Count));
            var currentPosition = corridorProperties.corridorStartPosition;
            corridor.Add(currentPosition);

            bool isValidCorridor = true;
            for (int i = 0; i < corridorProperties.corridorLength; i++)
            {
                currentPosition += direction;
                if (positionsToAvoid.Contains(currentPosition))
                {
                    isValidCorridor = false;
                    break;
                }
                corridor.Add(currentPosition);
            }

            if (isValidCorridor)
            {
                return corridor;
            }
            else
            {
                untriedDirections.Remove(direction);
            }
        }
        throw new System.Exception("No valid random walk corridor direction.");
    }

    public static HashSet<Vector2Int> CreateCorridors(CorridorProperties corridorProperties, HashSet<Vector2Int> positionsToAvoid)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        var currentPosition = corridorProperties.corridorStartPosition;
        for (int i = 0; i < corridorProperties.corridorCount; i++)
        {
            var corridor = SimpleRandomWalkCorridor(corridorProperties, positionsToAvoid);

            currentPosition = corridor[corridor.Count - 1];
            corridorProperties.corridorStartPosition = currentPosition;
            floorPositions.UnionWith(corridor);
        }

        return floorPositions;
    }

    public static HashSet<Vector2Int> GetCorridorDoors(CorridorProperties corridorProperties, HashSet<Vector2Int> corridorFloorPositions)
    {
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();

        foreach (var position in corridorFloorPositions)
        {
            Vector2Int differenceFromStart = position - corridorProperties.corridorStartPosition;
            //if position is at the end of a corridor unit
            if (differenceFromStart.x % corridorProperties.corridorLength == 0 && differenceFromStart.y % corridorProperties.corridorLength == 0)
            {
                foreach (var direction in Direction2D.cardinalDirectionsList)
                {
                    Vector2Int potentialDoor = position + direction;
                    if (!corridorFloorPositions.Contains(potentialDoor))
                    {
                        doorPositions.Add(potentialDoor);
                    }
                }
            }
        }

        return doorPositions;
    }

    public struct CorridorProperties
    {
        public Vector2Int corridorStartPosition;
        public int corridorLength;
        public int corridorCount;

        public CorridorProperties(Vector2Int corridorStartPosition, int corridorLength, int corridorCount)
        {
            this.corridorStartPosition = corridorStartPosition;
            this.corridorLength = corridorLength;
            this.corridorCount = corridorCount;
        }
    }
}
