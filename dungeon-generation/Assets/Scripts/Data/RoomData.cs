using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="RoomParameters_", menuName = "RoomData")]
public class RoomData : ScriptableObject
{
    public List<Vector2Int> doors;
    public List<BoundsInt> floors;
}
