using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomsDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    private RoomData startRoom;
    [SerializeField]
    private List<RoomData> roomParametersList;

    [SerializeField]
    private int numberOfRooms;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();

    protected override void RunProceduralGeneration()
    {
        Clear();
        for (int i = 0; i < numberOfRooms; i++)
        {
            AddNewRoom();
        }
        HashSet<Vector2Int> floorAndDoorPositions = new HashSet<Vector2Int>(floorPositions);
        floorAndDoorPositions.UnionWith(doorPositions);
        WallGenerator.CreateWalls(floorAndDoorPositions, tilemapVisualiser);
    }

    public void AddNewRoom()
    {
        if (floorPositions.Count > 0)
        {
            HashSet<Vector2Int> untriedDoors = new HashSet<Vector2Int>(doorPositions);
            while (untriedDoors.Count > 0)
            {
                Vector2Int randomTargetDoor = untriedDoors.ElementAt(Random.Range(0, untriedDoors.Count));

                HashSet<RoomData> untriedRoomDatas = new HashSet<RoomData>(roomParametersList);
                while (untriedRoomDatas.Count > 0)
                {
                    RoomData randomRoomData = untriedRoomDatas.ElementAt(Random.Range(0, untriedRoomDatas.Count));

                    HashSet<Vector2Int> untriedRoomDoors = new HashSet<Vector2Int>(randomRoomData.doors);
                    while (untriedRoomDoors.Count > 0)
                    {
                        Vector2Int randomRoomDoor = untriedRoomDoors.ElementAt(Random.Range(0, untriedRoomDoors.Count));
                        HashSet<Vector2Int> currentRoomFloorPositions = GetFloorPositions(randomRoomData.floors, Vector2Int.zero, 0);
                        (bool status, int rotation, Vector2Int translation) requiredTransformation = MapDoorToDoor(randomRoomDoor, currentRoomFloorPositions, randomTargetDoor);

                        if (requiredTransformation.status == true)
                        {
                            if (CreateRoom(randomRoomData, requiredTransformation.translation, requiredTransformation.rotation) == 0)
                            {
                                return;
                            }
                        }
                        untriedRoomDoors.Remove(randomRoomDoor);
                    }

                    untriedRoomDatas.Remove(randomRoomData);
                }

                untriedDoors.Remove(randomTargetDoor);
            }
        }
        else
        {
            CreateRoom(startRoom, Vector2Int.zero, 0);
        }
    }

    private (bool status, int rotation, Vector2Int translation) MapDoorToDoor(Vector2Int localDoorPosition, IEnumerable<Vector2Int> localFloorPositions, Vector2Int targetDoor)
    {
        (bool status, int rotation, Vector2Int translation) transformation = (false, 0, Vector2Int.zero);

        Vector2Int localDoorDirection = GetDoorDirection(localDoorPosition, localFloorPositions);
        Vector2Int targetDoorDirection = GetDoorDirection(targetDoor, floorPositions);

        if (localDoorDirection == Vector2Int.zero || targetDoor == Vector2Int.zero)
        {
            return transformation;
        }

        int requiredRotation = Direction2D.GetAngleBetweenVectors(localDoorDirection, -targetDoorDirection);
        print("     local door dir, target door dir = " + localDoorDirection.ToString() + "  " + targetDoorDirection.ToString());
        print("     local to -target = " + requiredRotation.ToString());

        Vector2Int newDoorPosition = targetDoor + targetDoorDirection;
        Vector2Int rotatedLocalDoorPosition = Direction2D.Rotate(localDoorPosition, requiredRotation);
        Vector2Int requiredTranslation = newDoorPosition - rotatedLocalDoorPosition;

        transformation.rotation = requiredRotation;
        transformation.translation = requiredTranslation;

        transformation.status = true;
        return transformation;
    }

    private Vector2Int GetDoorDirection(Vector2Int door, IEnumerable<Vector2Int> floorPositionsToConsider)
    {
        foreach (var direction in Direction2D.cardinalDirectionsList)
        {
            if (floorPositionsToConsider.Contains(door + direction))
            {
                return direction * -1;
            }
        }
        return Vector2Int.zero;
    }

    private int CreateRoom(RoomData roomData, Vector2Int roomStartPosition, int roomRotation)
    {
        HashSet<Vector2Int> floor = GetFloorPositions(roomData.floors, roomStartPosition, roomRotation);
        HashSet<Vector2Int> doors = GetDoorPositions(roomData.doors, roomStartPosition, roomRotation);

        if (floorPositions.Overlaps(floor))
        {
            return -1;
        }
        floorPositions.UnionWith(floor);
        doorPositions.UnionWith(doors);

        tilemapVisualiser.PaintFloorTiles(floor);
        tilemapVisualiser.PaintDoorTiles(doors);
        return 0;
    }

    public override void Clear()
    {
        floorPositions.Clear();
        doorPositions.Clear();
        tilemapVisualiser.Clear();
    }

    private HashSet<Vector2Int> GetDoorPositions(IEnumerable<Vector2Int> localDoorPositions, Vector2Int roomStartPosition, int roomRotation)
    {
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        foreach (var door in localDoorPositions)
        {
            if (roomRotation != 0)
            {
                Vector2Int rotatedPosition = Direction2D.Rotate(door, roomRotation);
                doorPositions.Add(roomStartPosition + rotatedPosition);
            }
            else
            {
                doorPositions.Add(roomStartPosition + door);
            }
        }
        return doorPositions;
    }

    private HashSet<Vector2Int> GetFloorPositions(IEnumerable<BoundsInt> boundsList, Vector2Int roomStartPosition, int roomRotation)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var bound in boundsList)
        {
            for (int row = 0; row < bound.size.y; row++)
            {
                for (int col = 0; col < bound.size.x; col++)
                {
                    Vector2Int position = (Vector2Int)bound.min + new Vector2Int(col, row);
                    if (roomRotation != 0)
                    {
                        Vector2Int rotatedPosition = Direction2D.Rotate(position, roomRotation);
                        floor.Add(roomStartPosition + rotatedPosition);
                    }
                    else
                    {
                        floor.Add(roomStartPosition + position);
                    }
                }
            }
        }
        return floor;
    }

}
