using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAi : MonoBehaviour
{
    public static EnemyAi Instance { get; private set; }
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer = 1f;

    [SerializeField] List<Transform> patrolPoints;
    private List<GridPosition> patrolGridPositions;
    private GridPosition currentPatrolRoute;
    private bool reachedCurrentRoute = false;

    private void Awake()
    {
        if (Instance != null)
        {
            // prevents duplicates
            Destroy(gameObject);
            return;
        }
        Instance = this;
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if(TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state) 
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:

                if(TryTakeEnemyAiAction(SetStateTakingTurn))
                {
                    state = State.Busy;
                }else
                {
                    //no more enemies that can take actions
                    state = State.WaitingForEnemyTurn;
                    TurnSystem.Instance.NextTurn();
                }
                break;
            case State.Busy:
                break;
        }

        
    }
    private void getPatrolGridPositions()
    {
        patrolGridPositions = new List<GridPosition>();
        foreach (Transform patrolPoint in patrolPoints)
        {
            GridPosition temp = GameManager.Instance.levelGrid.GetGridPosition(patrolPoint.position);
            patrolGridPositions.Add(temp);
        }
    }
    private void SetStateTakingTurn()
    {
        Debug.Log("Enemy Completed Action");
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 1f;
        }
        
    }

    private bool TryTakeEnemyAiAction(Action onEnemyAiActionComplete)
    {
        List<Unit> enemies = UnitManager.Instance.GetEnemyUnitList();
        
        foreach (Unit enemyUnit in enemies)
        {
            if (!enemyUnit.checkIfOpposedUnitInSight(GameManager.Instance.GetPlayer()))
            {
                if(Patrol(enemyUnit, onEnemyAiActionComplete))
                {
                    return true;
                }
            }
            if (TryTakeEnemyAiAction(enemyUnit, onEnemyAiActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAiAction(Unit enemyUnit, Action onEnemyAiActionComplete)
    {
        EnemyAiAction bestEnemyAiAction = null;
        BaseAction bestBaseAction = null;
        foreach(BaseAction baseAction in enemyUnit.GetActions())
        {
            //if we don't have enough action points to perform this action check the next
            if(!enemyUnit.CanSpendPointsToTakeAction(baseAction))
            {
                continue;
            }
            //if the action has no viable AiActions for us we check the next
            if(baseAction.GetBestEnemyAiAction() == null)
            {
                continue;
            }
            //if the actions best AiAction has a value of 0 we check the next action
            if(baseAction.GetBestEnemyAiAction().actionValue == 0)
            {
                continue;
            }
            if(bestEnemyAiAction == null)
            {
                bestEnemyAiAction = baseAction.GetBestEnemyAiAction();
                bestBaseAction = baseAction;
            }else
            {
                EnemyAiAction testEnemyAiAction = baseAction.GetBestEnemyAiAction();
                if(testEnemyAiAction != null && testEnemyAiAction.actionValue > bestEnemyAiAction.actionValue)
                {
                    bestEnemyAiAction = testEnemyAiAction;
                    bestBaseAction = baseAction;
                }
            }
         
        }
        //if we have a viable Ai action and we can spend the action points to perform it, we do
        if(bestEnemyAiAction != null && enemyUnit.TrySpendPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAiAction.gridPosition, onEnemyAiActionComplete);
            return true;
        /// otherwise we tell the system that this unit has no more viable actions to take
        }else
        {
            return false;
        }
    }

    private bool Patrol(Unit enemyUnit, Action onEnemyAiActionComplete)
    {
       
        getPatrolGridPositions();
        shufflePatrolGridPositions();
        GridPosition closestPatrolPoint = patrolGridPositions[0];
        GridPosition enemyPosition = enemyUnit.GetGridPosition();
        if (reachedCurrentRoute)
        {
            for (int x = 1; x < patrolGridPositions.Count; x++)
            {
                GridPosition tempGridPosition = patrolGridPositions[x];
                if (enemyPosition == tempGridPosition)
                {
                    reachedCurrentRoute = true;
                    continue;
                }
                if (GridPosition.Distance(enemyPosition, tempGridPosition) < GridPosition.Distance(enemyPosition, closestPatrolPoint))
                {
                    closestPatrolPoint = tempGridPosition;
                }
            }
            currentPatrolRoute = closestPatrolPoint;
            reachedCurrentRoute = false;
        }
        

        List<GridObject> pathToPatrolPoint = GameManager.Instance.pathfinding.FindPath(enemyPosition, closestPatrolPoint);
        if (pathToPatrolPoint.Count - 2 > enemyUnit.GetMaxMoveDistance())
        {
            for (int x = pathToPatrolPoint.Count; x > enemyUnit.GetMaxMoveDistance(); x--)
                {
                    pathToPatrolPoint.RemoveAt(enemyUnit.GetMaxMoveDistance());
                }
        }
        if(enemyUnit.TrySpendPointsToTakeAction(enemyUnit.GetAction<MoveAction>()))
        {
            enemyUnit.GetAction<MoveAction>().TakeAction(pathToPatrolPoint[pathToPatrolPoint.Count - 1].GetGridPosition(), onEnemyAiActionComplete);
            if (enemyPosition == currentPatrolRoute)
            {
                reachedCurrentRoute = true;
            }
            return true;
        }
        return false;
    }

    private void shufflePatrolGridPositions()
    {
        for (int i = 0; i < patrolGridPositions.Count; i++)
        {
            GridPosition temp = patrolGridPositions[i];
            int randomIndex = UnityEngine.Random.Range(i, patrolGridPositions.Count);
            patrolGridPositions[i] = patrolGridPositions[randomIndex];
            patrolGridPositions[randomIndex] = temp;
        }
    }

    
}
