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
    
    //Contructor takes a function as a parameter so that we can use the Grid for several object types
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int,TGridObject> createGridObject )
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        //stores the position of our grids (0,0) in the games world space
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0);x++)
        {
            for(int z = 0; z < gridArray.GetLength(1);z++)
            {
                gridArray[x, z] = createGridObject(this,x,z);
            }
        }
    }



    // Converts a given grid position into a corresponding Vector 3
    public Vector3 GetWorldPosition(GridPosition gridPos)
    {
        return new Vector3(gridPos.x,originPosition.y, gridPos.y) * cellSize + originPosition ;
    }
    // Converts a given x and z position into a corresponding Vector 3
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, originPosition.y, z) * cellSize + originPosition;
    }

    // Converts a given Vector 3 into a corresponding grid position
    public GridPosition GetGridPosition(Vector3 worldPos)
    {
        return new GridPosition(
            Mathf.FloorToInt((worldPos - originPosition).x / cellSize),
            Mathf.FloorToInt((worldPos - originPosition).z / cellSize)
        );
    }

    // returns object residing in given grid position
    public TGridObject GetGridObject(GridPosition gridPos)
    {
        return gridArray[gridPos.x, gridPos.y];
    }

    // returns object residing in given x and z position, this version uses separate x and y values so that we can iterate with loops
    public TGridObject GetGridObject(int x, int z)
    {
        return gridArray[x, z];
    }

    //checks if the given worldPos is on our grid returns true if it is, false if not
    public bool isOnGrid(Vector3 worldPos)
    {
        if(worldPos.x < 0 || worldPos.x > width*cellSize || worldPos.z < 0 || worldPos.z > height*cellSize)
        {
            return false;
        }
        return true;
    }

    public bool isOnGrid(GridPosition gridPos)
    {
        if (gridPos.x < 0 || gridPos.x > width * cellSize || gridPos.y < 0 || gridPos.y > height * cellSize)
        {
            return false;
        }
        return true;
    }


}
