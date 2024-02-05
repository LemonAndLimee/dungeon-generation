using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualiser tilemapVisualiser = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        Clear();
        RunProceduralGeneration();
    }

    public virtual void Clear()
    {
        tilemapVisualiser.Clear();
    }

    protected abstract void RunProceduralGeneration();
}
