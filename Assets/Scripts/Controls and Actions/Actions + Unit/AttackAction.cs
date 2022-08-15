using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    private Unit target;
    private bool attacking = false;
    private bool isLerping = false;
    private float timer = 0f;
    private int attackRange = 2;

    public event EventHandler<OnAttackEventArgs> onAttack;

    public class OnAttackEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private void Update()
    {
        HandleAttacking();
    }

    public void HandleAttacking()
    {
        if (attacking)
        {
            if (isLerping)
            {
                timer += Time.deltaTime * unit.GetMoveSpeed();
                //gives us the direction to our target
                Vector3 aimDirection = (target.GetWorldPosition() - transform.position).normalized;

                //makes rotation smoother
                float rotationAdjustment = 3f;
                //points unit in the direction of the target
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, timer/ rotationAdjustment);
                
                if (timer >= 1.0f)
                {
                    isLerping = false;
                    //we have finshed turning to the target
                    //hit target, validation happened in GetValidActionGridPositions
                    target.Damage(50);
                    onAttack?.Invoke(this, new OnAttackEventArgs {targetUnit = target, shootingUnit = unit });
                    timer = 0.0f;
                    ActionComplete();
                }
            }
        }
    }
    public override string GetActionName()
    {
        return "shoot";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        GridPosition unitGridPosition = unit.gridPosition;
        return GetValidActionGridPositions(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositions(GridPosition unitGridPosition)
    {
        List<GridPosition> validPositions = new List<GridPosition>();

        for (int x = -attackRange; x <= attackRange; x++)
        {
            for (int z = -attackRange; z <= attackRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition positionToCheck = offsetGridPosition + unitGridPosition;
                //we dont want to attack our own unit
                if (positionToCheck == unitGridPosition)
                {
                    continue;
                }
                //checks position is on grid 
                if (!GameManager.Instance.levelGrid.isOnGrid(positionToCheck))
                {
                    continue;
                }
                //checks position contains a unit
                if (!GameManager.Instance.levelGrid.GetGridObject(positionToCheck).HasUnit())
                {
                    continue;
                }
                target = GameManager.Instance.levelGrid.GetGridObject(positionToCheck).GetUnit();
                //dont want to be able to attack friendly units
                if (target.IsEnemy() == unit.IsEnemy())
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
        isLerping = true;
        attacking = true;
        target = GameManager.Instance.levelGrid.GetGridObject(gridPosition).GetUnit();
        

    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    public override EnemyAiAction GetEnemyAiAction(GridPosition gridPosition)
    {
        return new EnemyAiAction
        {
            gridPosition = gridPosition,
            ActionValue = 100,
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositions(gridPosition).Count;
    }

}
