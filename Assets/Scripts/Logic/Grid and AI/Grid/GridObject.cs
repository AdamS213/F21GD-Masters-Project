using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private Grid<GridObject> parentGrid;
    private GridPosition gridPosition;

    public Unit unit;

    public int gCost;
    public int hCost;
    public int fCost;

    public GridObject cameFromNode;
    public List<GridObject> neighbors;

    public GridObject(Grid<GridObject> parentGrid, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.parentGrid = parentGrid;
    }


    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public Unit GetUnit()
    {
        return unit;
    }

    public void SetUnit(Unit unit)
    {
        if(!HasUnit())
        {
            this.unit = unit;
        }
            
    }

    public void ClearUnit()
    {
        this.unit = null;
        
    }

    public bool HasUnit()
    {
        return unit != null;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void setNeighbours(List<GridObject> neighbors)
    {
        this.neighbors = neighbors;
    }
}
