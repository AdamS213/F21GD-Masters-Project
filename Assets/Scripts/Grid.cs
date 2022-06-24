using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<TGridObject>
{

    public int height { get; }
    public int width { get; }
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;
    
    
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int,TGridObject> createGridObject )
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0);x++)
        {
            for(int y = 0; y < gridArray.GetLength(1);y++)
            {
                gridArray[x, y] = createGridObject(this,x,y);
            }
        }
    }
   

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,originPosition.y, y) * cellSize + originPosition ;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPosition).z / cellSize);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        return gridArray[x, y];
    }

    public bool isOnGrid(Vector3 worldPos)
    {
        if(worldPos.x < 0 || worldPos.x > width || worldPos.y < 0 || worldPos.y > height)
        {
            return false;
        }
        return true;
    }
}
