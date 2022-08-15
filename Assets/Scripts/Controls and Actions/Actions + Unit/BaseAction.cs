using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected bool isActive;
    protected Unit unit;
    protected Action onActionComplete;
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();

    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public abstract List<GridPosition> GetValidActionGridPositions();

    public virtual bool isValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validPositions = GetValidActionGridPositions();
        return validPositions.Contains(gridPosition);
    }

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
    }

    public EnemyAiAction GetBestEnemyAiAction()
    {
        List<EnemyAiAction> enemyAiActionList = new List<EnemyAiAction>();

        List<GridPosition> validActionPostionList = GetValidActionGridPositions();

        foreach (GridPosition gridPosition in validActionPostionList)
        {
            EnemyAiAction enemyAiAction = GetEnemyAiAction(gridPosition);
            enemyAiActionList.Add(enemyAiAction);
        }
        if (enemyAiActionList.Count > 0)
        {
            enemyAiActionList.Sort((EnemyAiAction a, EnemyAiAction b) => b.ActionValue - a.ActionValue);
            return enemyAiActionList[0];
        } else
        {
            //no possible actions
            return null;
        }

        
    }

    public abstract EnemyAiAction GetEnemyAiAction(GridPosition gridPosition);
}
