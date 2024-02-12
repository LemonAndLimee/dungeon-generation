using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CorridorGenerationAlgorithms;

public class MixedRoomsAndCorridorsDungeonGenerator : RoomsDungeonGenerator
{
    [SerializeField]
    private int corridorLength, corridorCount;

    protected override void RunProceduralGeneration()
    {
        Clear();

        GenerateNewRoom();
        AddCorridorSystemAtRandomPosition();

        for (int i = 0; i < numberOfRooms; i++)
        {
            GenerateNewRoom();
        }
        AddWalls();
    }

    private void AddCorridorSystemAtRandomPosition()
    {
        Vector2Int randomDoor = doorPositions.ElementAt(Random.Range(0, doorPositions.Count));
        Vector2Int doorDirection = GetDoorDirection(randomDoor, floorPositions);
        Vector2Int corridorStartPosition = randomDoor + doorDirection;

        HashSet<Vector2Int> takenPositions = GetExistingTakenPositions();
        CorridorProperties corridorProperties = new CorridorProperties(corridorStartPosition, corridorLength, corridorCount);

        HashSet<Vector2Int> corridorFloorPositions = CreateCorridors(corridorProperties, takenPositions);
        HashSet<Vector2Int> corridorDoorPositions = GetCorridorDoors(corridorProperties, corridorFloorPositions);

        PaintNewStructure(corridorFloorPositions, corridorDoorPositions);
        AddUsedDoorTile(randomDoor);
    }

    
}
