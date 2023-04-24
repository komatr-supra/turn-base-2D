using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridSystem<TNode> where TNode : GridData
{    
    private Vector2Int size;
    public int Width => size.x;
    public int Height => size.y;
    private TNode[,] _nodes;
    public TNode[,] GridData { get => _nodes; }
    private static Vector2Int[] offsetsStraight = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
    public GridSystem(Vector2Int gridSize, Func<Vector2Int ,TNode> node)
    {
        size = gridSize;

        _nodes = new TNode[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                _nodes[x, y] = node(position);
            }
        }
    }

    public TNode GetNode(Vector2Int gridPosition)
    {
        return _nodes[gridPosition.x, gridPosition.y];
    }
    public void SetNode(TNode node)
    {
        _nodes[node.gridPosition.x, node.gridPosition.y] = node;
    }

    public bool IsValid(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < Width && gridPosition.y < Height;
    }

    public bool TryGetNode(Vector2Int gridPosition, out TNode node)
    {
        node = default;
        if(IsValid(gridPosition))
        {
            node = GetNode(gridPosition);
            return true;
        }
        return false;
    }
    public List<TNode> GetNeighbours(GridData gridData)
    {
        List<TNode> returnedList = new();
        foreach (Vector2Int offset in offsetsStraight)
        {
            Vector2Int neighbourPosition = gridData.gridPosition + offset;

            if(!IsValid(neighbourPosition)) continue;

            returnedList.Add(GetNode(neighbourPosition));
        }
        return returnedList;
    }
}
public interface GridData
{
    public Vector2Int gridPosition{ get; }    
}

