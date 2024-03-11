using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray; //2 dimension array
    public GridSystem(
        int width, int height, float cellSize, 
        Func<GridSystem<TGridObject>, 
        GridPos, 
        TGridObject> createGridObject
        )
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPos gridPos = new GridPos(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPos);
            }
        }
    }
    public Vector3 GetWorldPos(GridPos gridPos)
    {
        return new Vector3(gridPos.x, 0, gridPos.z) * cellSize;
    }
    public GridPos GetGridPos(Vector3 worldPos)
    {
        return new GridPos(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
            );
    }
    public TGridObject GetGridObject(GridPos gridPos)
    {
        return gridObjectArray[gridPos.x, gridPos.z];
    }
    public void CreatDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPos gridPos = new GridPos(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPos(gridPos), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPos));
            }
        }
    }
    public bool IsValidGridPos(GridPos gridPos)
    {
        return gridPos.x >= 0 &&
            gridPos.z >= 0 &&
            gridPos.x < width &&
            gridPos.z < height;
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
}
