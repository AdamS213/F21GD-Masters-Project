using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private GridPosition gridPosition;
    private Grid<GridObject> parentGrid;
    public Unit unit;

    public GridObject(Grid<GridObject> parentGrid, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.parentGrid = parentGrid;
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    public void ClearUnit()
    {
        this.unit = null;
    }

    public bool HasUnit()
    {
        return unit != null;
    }
}
