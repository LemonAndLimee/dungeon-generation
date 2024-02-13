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

        int indexOnWhichToGenerateCorridors = Random.Range(0, numberOfRooms);
        for (int i = 0; i < numberOfRooms; i++)
        {
            if (i == indexOnWhichToGenerateCorridors) AddCorridorSystemAtRandomPosition();
            GenerateNewRoom();
        }
        AddWalls();
    }

    private void AddCorridorSystemAtRandomPosition()
    {
        HashSet<Vector2Int> untriedDoors = new HashSet<Vector2Int>(doorPositions);
        untriedDoors.ExceptWith(usedDoorPositions);

        HashSet<Vector2Int> takenPositions = GetExistingTakenPositions();

        while (untriedDoors.Count > 0)
        {
            Vector2Int randomDoor = untriedDoors.ElementAt(Random.Range(0, untriedDoors.Count));
            Vector2Int doorDirection = GetDoorDirection(randomDoor, floorPositions);
            Vector2Int corridorStartPosition = randomDoor + doorDirection;

            CorridorProperties corridorProperties = new CorridorProperties(corridorStartPosition, corridorLength, corridorCount, inclusiveLowerLimitY);

            try
            {
                HashSet<Vector2Int> corridorFloorPositions = CreateCorridors(corridorProperties, takenPositions);
                HashSet<Vector2Int> corridorDoorPositions = GetCorridorDoors(corridorProperties, corridorFloorPositions);

                PaintNewStructure(corridorFloorPositions, corridorDoorPositions);
                AddUsedDoorTile(randomDoor);

                return;
            }
            catch
            {
                untriedDoors.Remove(randomDoor);
            }
        }
        throw new System.Exception("No valid corridor system could be added.");
    }

    
}
