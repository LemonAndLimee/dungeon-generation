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

    private HashSet<Vector2Int> usedDoorPositions = new HashSet<Vector2Int>();

    private HashSet<PotentialRoomCombination> alreadyCheckedPotentialRooms = new HashSet<PotentialRoomCombination>();

    protected override void RunProceduralGeneration()
    {
        Clear();
        for (int i = 0; i < numberOfRooms; i++)
        {
            GenerateNewRoom();
        }
        AddWalls();
    }

    public override void Clear()
    {
        floorPositions.Clear();
        doorPositions.Clear();
        usedDoorPositions.Clear();
        tilemapVisualiser.Clear();
    }

    private void AddWalls()
    {
        HashSet<Vector2Int> floorAndDoorPositions = new HashSet<Vector2Int>(floorPositions);
        floorAndDoorPositions.UnionWith(usedDoorPositions);
        WallGenerator.CreateWalls(floorAndDoorPositions, tilemapVisualiser);
    }

    public void GenerateNewRoom()
    {
        if (floorPositions.Count > 0)
        {
            try
            {
                AddRandomRoomToMap();
                return;
            }
            catch
            {
                Debug.LogError("No new room can be added.");
            }
        }
        else
        {
            PaintNewRoom(startRoom, new RoomTransformation(0, Vector2Int.zero));
        }   
    }

    private void AddRandomRoomToMap()
    {
        HashSet<Vector2Int> untriedDoors = new HashSet<Vector2Int>(doorPositions);
        untriedDoors.ExceptWith(usedDoorPositions);
        while (untriedDoors.Count > 0)
        {
            Vector2Int randomTargetDoor = untriedDoors.ElementAt(Random.Range(0, untriedDoors.Count));
            try
            {
                AddRandomRoomToDoorOnMap(randomTargetDoor);
                return;
            }
            catch
            {
                untriedDoors.Remove(randomTargetDoor);
            }
        }
        throw new System.Exception("No valid space for a new room.");
    }

    private void AddRandomRoomToDoorOnMap(Vector2Int mapDoor)
    {
        HashSet<RoomData> untriedRoomDatas = new HashSet<RoomData>(roomParametersList);
        while (untriedRoomDatas.Count > 0)
        {
            RoomData randomRoomData = untriedRoomDatas.ElementAt(Random.Range(0, untriedRoomDatas.Count));
            PotentialRoomCombination roomCombination = new PotentialRoomCombination(mapDoor, randomRoomData);
            if (!alreadyCheckedPotentialRooms.Contains(roomCombination))
            {
                try
                {
                    AddRandomRoomToMapFromRoomData(mapDoor, randomRoomData);
                    return;
                }
                catch
                {
                    alreadyCheckedPotentialRooms.Add(roomCombination);
                }
            }

            untriedRoomDatas.Remove(randomRoomData);
        }
        throw new System.Exception("No valid RoomData found.");
    }

    private void AddRandomRoomToMapFromRoomData(Vector2Int mapDoor, RoomData roomData)
    {
        HashSet<Vector2Int> untriedRoomDoors = new HashSet<Vector2Int>(roomData.doors);
        while (untriedRoomDoors.Count > 0)
        {
            Vector2Int randomRoomDoor = untriedRoomDoors.ElementAt(Random.Range(0, untriedRoomDoors.Count));

            RoomTransformation requiredTransformation = MapDoorToDoor(randomRoomDoor, roomData, mapDoor);
            HashSet<Vector2Int> transformedRoomFloorPositions = GetRoomFloorPositions(roomData.floors, requiredTransformation);
            if (CheckIfNewRoomOverlaps(transformedRoomFloorPositions) == true)
            {
                untriedRoomDoors.Remove(randomRoomDoor);
            }
            else
            {
                PaintNewRoom(roomData, requiredTransformation);
                AddUsedDoorTile(mapDoor);
                return;
            }
        }
        throw new System.Exception("No valid room door found.");
    }

    private bool CheckIfNewRoomOverlaps(HashSet<Vector2Int> roomFloor)
    {
        HashSet<Vector2Int> takenSpaces = GetExistingTakenPositions();
        if (takenSpaces.Overlaps(roomFloor)) return true;
        else return false;
    }

    private HashSet<Vector2Int> GetExistingTakenPositions()
    {
        HashSet<Vector2Int> takenSpaces = WallGenerator.FindWalls(floorPositions, Direction2D.cardinalDirectionsList);
        takenSpaces.UnionWith(floorPositions);
        return takenSpaces;
    }

    private void PaintNewRoom(RoomData roomData, RoomTransformation roomTransformation)
    {
        HashSet<Vector2Int> floor = GetRoomFloorPositions(roomData.floors, roomTransformation);
        HashSet<Vector2Int> doors = GetRoomDoorPositions(roomData.doors, roomTransformation);

        floorPositions.UnionWith(floor);
        doorPositions.UnionWith(doors);

        tilemapVisualiser.PaintFloorTiles(floor);
    }

    private void AddUsedDoorTile(Vector2Int door)
    {
        usedDoorPositions.Add(door);
        tilemapVisualiser.PaintDoorTiles(usedDoorPositions);
    }

    private RoomTransformation MapDoorToDoor(Vector2Int localDoorPosition, RoomData roomData, Vector2Int targetDoor)
    {
        RoomTransformation transformation = new RoomTransformation();
        Vector2Int localDoorDirection, targetDoorDirection;
        HashSet<Vector2Int> localFloorPositions = GetRoomFloorPositions(roomData.floors, new RoomTransformation(0, Vector2Int.zero));
        try
        {
            localDoorDirection = GetDoorDirection(localDoorPosition, localFloorPositions);
            targetDoorDirection = GetDoorDirection(targetDoor, floorPositions);
        }
        catch
        {
            throw new System.Exception("Both doors must be connected to a floor.");
        }


        int requiredRotation = Direction2D.GetAngleBetweenVectors(localDoorDirection, -targetDoorDirection);

        Vector2Int newDoorPosition = targetDoor;
        Vector2Int rotatedLocalDoorPosition = Direction2D.Rotate(localDoorPosition, requiredRotation);
        Vector2Int requiredTranslation = newDoorPosition - rotatedLocalDoorPosition;

        transformation.rotation = requiredRotation;
        transformation.translation = requiredTranslation;

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
        throw new System.Exception("Door is not connected to a floor.");
    }

    private HashSet<Vector2Int> GetRoomDoorPositions(IEnumerable<Vector2Int> localDoorPositions, RoomTransformation roomTransformation)
    {
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        foreach (var door in localDoorPositions)
        {
            if (roomTransformation.rotation != 0)
            {
                Vector2Int rotatedPosition = Direction2D.Rotate(door, roomTransformation.rotation);
                doorPositions.Add(roomTransformation.translation + rotatedPosition);
            }
            else
            {
                doorPositions.Add(roomTransformation.translation + door);
            }
        }
        return doorPositions;
    }

    private HashSet<Vector2Int> GetRoomFloorPositions(IEnumerable<BoundsInt> boundsList, RoomTransformation roomTransformation)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var bound in boundsList)
        {
            for (int row = 0; row < bound.size.y; row++)
            {
                for (int col = 0; col < bound.size.x; col++)
                {
                    Vector2Int position = (Vector2Int)bound.min + new Vector2Int(col, row);
                    if (roomTransformation.rotation != 0)
                    {
                        Vector2Int rotatedPosition = Direction2D.Rotate(position, roomTransformation.rotation);
                        floor.Add(roomTransformation.translation + rotatedPosition);
                    }
                    else
                    {
                        floor.Add(roomTransformation.translation + position);
                    }
                }
            }
        }
        return floor;
    }

}

struct RoomTransformation
{
    public int rotation;
    public Vector2Int translation;

    public RoomTransformation(int rotation, Vector2Int translation)
    {
        this.rotation = rotation;
        this.translation = translation;
    }
}

// used for storing combinations already tried
struct PotentialRoomCombination
{
    public Vector2Int doorConnectedTo;
    public RoomData roomData;

    public PotentialRoomCombination(Vector2Int doorConnectedTo, RoomData roomData)
    {
        this.doorConnectedTo = doorConnectedTo;
        this.roomData = roomData;
    }
}