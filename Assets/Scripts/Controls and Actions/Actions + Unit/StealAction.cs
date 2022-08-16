using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealAction : BaseAction
{

    private Unit target;

    public static event EventHandler onSteal;
    public override string GetActionName()
    {
        return "Steal";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        for(int x = -1; x<=1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {

                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition positionToCheck = offsetGridPosition + unit.GetGridPosition();
                //dont want to be able to steal from ourselves
                if (positionToCheck == unit.GetGridPosition())
                {
                    continue;
                }
                //checks position is on grid 
                if (!GameManager.Instance.levelGrid.isOnGrid(positionToCheck))
                {
                    continue;
                }
                validPositions.Add(positionToCheck);
            }
        }
        return validPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        if(GameManager.Instance.levelGrid.GetGridObject(gridPosition).GetUnit(out target))
        {
            
            if(target.IsLootable())
            {
                
                onSteal.Invoke(this, EventArgs.Empty);
                ActionComplete();
                return;
            }
        }
        ActionComplete();
    }
}
