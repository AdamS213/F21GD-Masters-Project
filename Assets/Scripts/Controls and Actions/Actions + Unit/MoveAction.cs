using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler onStartMoving;
    public event EventHandler onStopMoving;
    public List<Vector3> currentPath { get; private set; }
    private int currentPathIndex;
    private bool isLerping = false;
    
    private float timer;
    private float moveSpeed;
    private int maxMoveDistance;


    protected override void Awake()
    {
        base.Awake();
        moveSpeed = unit.GetMoveSpeed();
        maxMoveDistance = unit.GetMaxMoveDistance();
    }
    private void Update()
    {
        if (isActive)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (currentPath != null)
        {
            if (!isLerping)
            {

                if (currentPathIndex != currentPath.Count - 1)
                {
                    isLerping = true;
                }
                else
                {
                    currentPath = null;
                    GridVisual.Instance.UpdateGridVisual();
                    currentPathIndex = 0;
                    onStopMoving?.Invoke(this, EventArgs.Empty);
                    ActionComplete();
                }
            }
            else
            {
                timer += Time.deltaTime * moveSpeed;

                //gives us the direction we are moving in
                Vector3 moveDirection = (currentPath[currentPathIndex + 1] - currentPath[currentPathIndex]).normalized;

                //makes rotation smoother
                float rotationAdjustment = 3f;
                //points unit in the direction it is moving
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, timer/rotationAdjustment);
                //makes movement from point a to b smooth
                transform.position = Vector3.Lerp(currentPath[currentPathIndex], currentPath[currentPathIndex + 1], timer);
                // should make camera follow player during movement
                GameManager.Instance.camCont.FocusCameraOnPoint(transform.position + Vector3.down);
                if (timer >= 1.0f)
                {
                    isLerping = false;
                    
                    currentPathIndex++;
                    timer = 0.0f;
                }
            }
        }
    }

    public override void TakeAction(GridPosition targetPosition,Action onActionComplete)
    {
        ActionStart(onActionComplete);
        //avoids error caused by clicking the square the player occupies
        Vector3 targetWorldPosition = GameManager.Instance.levelGrid.GetWorldPosition(targetPosition);
            
        List<Vector3> tempPath = GameManager.Instance.pathfinding.FindPath(transform.position, targetWorldPosition);
        if(tempPath == null)
        {
            ActionComplete();
            return;
        }
        
        if (tempPath.Count <= 1 )
        {
            ActionComplete();
            return;
        }

        onStartMoving?.Invoke(this, EventArgs.Empty);
        currentPathIndex = 0;
        isLerping = true;
        currentPath = tempPath;
        
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        

        List<GridPosition> validPositions = new List<GridPosition>();

        List <GridPosition> neighborPositions = GameManager.Instance.levelGrid.GetGridObject(unit.GetGridPosition()).GetNeighbourPositions();



        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition positionToCheck = offsetGridPosition + unit.GetGridPosition();
                //don't want to try and move to the position we already occupy
                if (positionToCheck == unit.GetGridPosition())
                {
                    continue;
                }
                //checks position is on grid 
                if (!GameManager.Instance.levelGrid.isOnGrid(positionToCheck))
                {
                    continue;
                }
                //checks position doesnt already contain a unit
                if (GameManager.Instance.levelGrid.GetGridObject(positionToCheck).HasUnit())
                {
                    continue;
                }
                //checks that the path to reach the position would not be larger than our movement range
                List<GridObject> tempPath = GameManager.Instance.pathfinding.FindPath(unit.GetGridPosition(),positionToCheck);
                if (tempPath == null)
                {
                    continue;
                }
                //plus two because the path includes our destination and our position
                if (tempPath.Count >= maxMoveDistance+2)
                {
                    continue;
                }
                validPositions.Add(positionToCheck);
            }
        }
        return validPositions;
    }

    

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAiAction GetEnemyAiAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<AttackAction>().GetTargetCountAtPosition(gridPosition);

        //check if our current position is a firing position, if so we don't want to move
        if(unit.GetAction<AttackAction>().GetTargetCountAtPosition(unit.GetGridPosition()) > 0)
        {
            return new EnemyAiAction
            {
                gridPosition = gridPosition,
                actionValue = 0

            };
        } 
        // if we aren't in a spot where we can hit something we want to move somewhere we can
        else
        {
            return new EnemyAiAction
            {
                gridPosition = gridPosition,
                actionValue = (targetCountAtGridPosition * 10)+1

            };
        }
    }

    
}
