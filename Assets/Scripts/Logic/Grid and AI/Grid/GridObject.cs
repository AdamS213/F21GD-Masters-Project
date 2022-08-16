using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private Grid<GridObject> parentGrid;
    private GridPosition gridPosition;

    private Unit unit;
    private bool isWalkable = true;

    public int gCost;
    public int hCost;
    public int fCost;

    public GridObject cameFromNode;
    private List<GridObject> neighbors;

    public GridObject(Grid<GridObject> parentGrid, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.parentGrid = parentGrid;
    }


    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public bool GetUnit(out Unit outUnit)
    {
        outUnit = this.unit;
        
        return HasUnit();
    }

    public void SetUnit(Unit unit)
    {
        
        if(isWalkable)
        {
            this.unit = unit;
            SetIsWalkable(false);
        }
            
    }

    public void ClearUnit()
    {
        this.unit = null;
        SetIsWalkable(true);
    }

    public bool HasUnit()
    {
        return unit != null;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetNeighbours(List<GridObject> neighbors)
    {
        this.neighbors = neighbors;
    }

    public List<GridObject> GetNeighbours()
    {
        return neighbors;
    }

    public List<GridPosition> GetNeighbourPositions()
    {
        List<GridPosition> gridPositions = new List<GridPosition>();

        foreach(GridObject neighbor in neighbors)
        {
            gridPositions.Add(neighbor.GetGridPosition());
        }
        return gridPositions;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }
}
