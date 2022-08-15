using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    float totalSpinAddAmount;

    public override void TakeAction(GridPosition gridPosition,Action onActionComplete)
    {
        ActionStart(onActionComplete);
        totalSpinAddAmount = 0;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAddAmount += spinAddAmount;
        if(totalSpinAddAmount >= 360)
        {
            ActionComplete();
        }
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        GridPosition unitGridPosition = unit.gridPosition;

        return new List<GridPosition> { unitGridPosition };

    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAiAction GetEnemyAiAction(GridPosition gridPosition)
    {
        return new EnemyAiAction
        {
            gridPosition = gridPosition,
            ActionValue = 0,
        };
    }
}
